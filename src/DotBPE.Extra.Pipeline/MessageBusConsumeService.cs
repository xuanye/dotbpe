using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DotBPE.Baseline.Extensions;
using DotBPE.Rpc;
using DotBPE.Rpc.BestPractice;
using DotBPE.Rpc.Client;
using Foundatio.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;

namespace DotBPE.Extra
{
    public class MessageBusConsumeService : IHostedService
    {

        private readonly IMessageBus _messageBus;
        private readonly IJsonParser _jsonParser;
        private readonly IDBStorage _dbStorage;
        private readonly IRpcInvokerReflection _rpcServiceScanner;
        private readonly ILogger<MessageBusConsumeService> _logger;
        private IPipelineTaskService _pipelineTaskService;

        private PipelineOptions _pipelineOptions;
        private int DefaultRpcTimeout = 10 * 1000; //10秒

        public MessageBusConsumeService(IMessageBus messageBus, IOptions<PipelineOptions> optionsAccessor, IPipelineTaskService pipelineTaskService,
            IJsonParser jsonParser,
            IDBStorage dbStorage,
            IRpcInvokerReflection rpcServiceScanner,
            ILogger<MessageBusConsumeService> logger)
        {
            _pipelineOptions = optionsAccessor.Value;
            _pipelineTaskService = pipelineTaskService;
            this._messageBus = messageBus;
            this._jsonParser = jsonParser;
            this._dbStorage = dbStorage;

            this._rpcServiceScanner = rpcServiceScanner;
            this._logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            this._messageBus.SubscribeAsync<QueueTaskItem>(Consume, cancellationToken);

            this._logger.LogInformation("MessageBusConsumeService is Started!");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this._messageBus.Dispose();
            this._logger.LogInformation("MessageBusConsumeService is Stop!");
            return Task.CompletedTask;
        }

        private async Task Consume(QueueTaskItem item)
        {
            this._logger.LogDebug("serviceId={serviceId},messageId={messageId},data = {data}", item.ServiceId, item.MessageId, item.ExecuteJson);
            var res = await InvokeCall(item);
            if (res.Code == 0) return;
            //this._logger.LogWarning("执行失败{code},returnMessage={returnMessage}:serviceId={serviceId},messageId={messageId},data = {data}", res.Code, returnMessage, item.ServiceId, item.MessageId, item.ExecuteJson);

            if (IsRetryException(res) == false)
            {
                if (item.TaskId > 0) //延时任务
                    await this._dbStorage.Failed(item.TaskId, 10000, GetMaxTimeSpan());//业务异常不需要重试，所以把失败次数改大一点
                return;
            }

            await ProcessInnerError(item, res);
        }

        private async Task<RpcResult> InvokeCall(QueueTaskItem item)
        {
            var res = new RpcResult();
            var serviceId = item.ServiceId;
            var messageId = item.MessageId;
            var json = item.ExecuteJson;
            try
            {
                var rpcMethod = this._rpcServiceScanner.GetRpcInvoker(serviceId, messageId);
                var parameters = rpcMethod.InvokeMethod.GetParameters();

                var param0 = this._jsonParser.FromJson(json, parameters[0].ParameterType);
                var callParams = parameters.Length == 1 ?
                    new[] { param0 }
                    : new[] { param0, parameters[1].HasDefaultValue ? parameters[1].DefaultValue : DefaultRpcTimeout };

                var retVal = rpcMethod.InvokeMethod.Invoke(rpcMethod.ServiceInstance, callParams);

                var retValType = retVal.GetType();
                if (retValType == typeof(Task))
                {
                    return res;
                }

                var tType = retValType.GenericTypeArguments[0];
                if (tType == typeof(RpcResult))
                {
                    Task<RpcResult> retTask = retVal as Task<RpcResult>;
                    var result = await retTask;
                    res.Code = result.Code;
                }
                else if (tType.IsGenericType)
                {
                    Task retTask = retVal as Task;
                    await retTask.AnyContext();

                    var resultProp = retValType.GetProperty("Result");
                    if (resultProp == null)
                    {
                        res.Code = RpcErrorCodes.CODE_INTERNAL_ERROR;
                        return res;
                    }

                    object result = resultProp.GetValue(retVal);

                    var codeProp = tType.GetProperty("Code");
                    if (codeProp != null)
                    {
                        res.Code = (int)codeProp.GetValue(result);
                    }
                    if (res.Code != 0)
                    {
                        var returnMessage = GetReturnMessage(result) ?? string.Empty;
                        this._logger.LogWarning("执行失败{code},returnMessage={returnMessage}:serviceId={serviceId},messageId={messageId},data = {data}", res.Code, returnMessage, item.ServiceId, item.MessageId, item.ExecuteJson);
                    }
                }
            }
            catch (Exception ex)
            {
                res.Code = RpcErrorCodes.CODE_INTERNAL_ERROR;
                this._logger.LogError(ex, "invoke queue task fail:" + ex.Message);
            }

            return res;
        }

        /// <summary>
        /// 处理内部异常 进行重试
        /// </summary>
        /// <param name="item"></param>
        /// <param name="res"></param>
        /// <returns></returns>
        private async Task ProcessInnerError(QueueTaskItem item, RpcResult res)
        {
            if (IsRetryException(res) == false) return;

            //if (item.FailCount == 0) //第一次立即重试
            //{
            //    await _messageBus.PublishAsync(item);
            //    return;
            //}

            if (item.TaskId > 0) //延时任务
            {
                var nextExecuteTime = GetNextRetryTimeSpan(item.FailCount + 1);
                if (item.FailCount >= this._pipelineOptions.RetryMaxCount) nextExecuteTime = GetMaxTimeSpan();
                await this._dbStorage.Failed(item.TaskId, 1, nextExecuteTime); //要更新下次执行延时时间，不然就立即执行了
                return;
            }
            else
            { //即时任务第一次失败，插入一条延时任务
                var failCount = item.FailCount + 1;
                var delayTask = new DelayTask
                {
                    ServiceId = item.ServiceId,
                    MessageId = item.MessageId,
                    ExecuteJson = item.ExecuteJson,
                    ExecuteTime = DateTime.Now.Add(GetNextRetryTimeSpan(failCount)),
                    TaskCode = null,
                    Status = 1,
                    FailCount = failCount,
                    CheckCode = null,
                    Description = "即时任务失败转延时任务重试",
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    RouteKey = item.RouteKey
                };
                await this._pipelineTaskService.EnqueueDelay(delayTask);
                return;
            }
        }

        /// <summary>
        /// 是否是重试异常（需要重试的异常） 目前处理：服务内部系统异常，服务网络断开。
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        private bool IsRetryException(RpcResult res)
        {
            //return res != null && res.Code == RpcErrorCodes.CODE_INTERNAL_ERROR;
            return res != null && this._pipelineOptions.RetryExceptionCode.ToList().Contains(res.Code) && this._pipelineOptions.RetryMaxCount > 0;
        }

        /// <summary>
        /// 根据失败次数 计算下次重试时间
        /// </summary>
        /// <param name="failCount"></param>
        /// <returns></returns>
        private TimeSpan GetNextRetryTimeSpan(int failCount)
        {
            var retryStrategy = this._pipelineOptions.RetryStrategy;
            if (failCount <= 0) failCount = 1;
            if (failCount > retryStrategy.Length)
            {
                failCount = retryStrategy.Length;
            }
            return TimeSpan.FromSeconds(retryStrategy[failCount - 1]);


            //var rand = new Random(Guid.NewGuid().GetHashCode());
            //var nextTry = rand.Next((int)Math.Pow(failCount, 2), (int)Math.Pow(failCount + 1, 2) + 1);
            //return TimeSpan.FromMinutes(nextTry);

        }

        private TimeSpan GetMaxTimeSpan()
        {
            var now = DateTime.Now;
            return now.AddYears(100) - now;
        }

        private static string GetReturnMessage(object rpcResult)
        {
            string returnMessage = null;
            if (rpcResult == null) return returnMessage;
            //RpcResult rpcResult = new RpcResult<VoidRes> { Data = new VoidRes { ReturnMessage = "错了" } };
            var dataProperty = rpcResult.GetType().GetProperty("Data");
            var data = dataProperty?.GetValue(rpcResult);
            if (data != null)
            {
                returnMessage = data.GetType().GetProperty("ReturnMessage")?.GetValue(data)?.ToString();
            }

            return returnMessage;
        }
    }
}
