using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using DotBPE.Rpc;
using Foundatio.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotBPE.Extra
{
    public class DefaultPipelineTaskService : IPipelineTaskService
    {
        private readonly IDBStorage _dbStorage;
        private readonly IRedisStorage _redisStorage;
        private readonly IMessageBus _messageBus;
        private readonly IJsonParser _jsonParser;
        private readonly ILogger<DefaultPipelineTaskService> _logger;
        private readonly PipelineOptions _options;

        public DefaultPipelineTaskService(
            IOptions<PipelineOptions> optionsAccessor,
            IRedisStorage redisStorage,
            IDBStorage dbStorage,
            IMessageBus messageBus,
            IJsonParser jsonParser,
            ILogger<DefaultPipelineTaskService> logger)
        {
            _dbStorage = dbStorage;
            _redisStorage = redisStorage;
            _messageBus = messageBus;
            _jsonParser = jsonParser;
            _logger = logger;
            _options = optionsAccessor?.Value ?? new PipelineOptions();
        }

        public Task<RpcResult> Enqueue(int serviceId, ushort messageId, string executeJson)
        {
            return EnqueueEx(serviceId, messageId, executeJson, null);
            //RpcResult result = new RpcResult();

            //try
            //{
            //    QueueTaskItem item = new QueueTaskItem(serviceId, messageId, executeJson);
            //    await this._messageBus.PublishAsync(item);
            //}
            //catch (Exception e)
            //{
            //    this._logger.LogError(e, "enqueue task fail:" + e.Message);
            //    result.Code = RpcErrorCodes.CODE_INTERNAL_ERROR;
            //}

            //return result;
        }

        public Task<RpcResult> Enqueue<T>(Func<T, Task> delayFunc, T arg1) where T : class
        {

            var interfaces = delayFunc.Method.DeclaringType.FindInterfaces((t, o) => t.GetCustomAttribute<RpcServiceAttribute>() != null, null);
            if (interfaces.Length != 1)
            {
                throw new NotSupportedException("不支持的服务调用,服务必须是具备RpcServiceAttribute的接口实现类");
            }

            var serviceType = interfaces[0];
            var method = serviceType.GetMethod(delayFunc.Method.Name);
            var sAttr = serviceType.GetCustomAttribute<RpcServiceAttribute>();
            var mAttr = method.GetCustomAttribute<RpcMethodAttribute>();
            if (sAttr == null)
            {
                throw new NotSupportedException($"不支持的服务调用，服务类型{serviceType.Name}");
            }

            if (mAttr == null)
            {
                throw new NotSupportedException($"不支持的方法调用，方法名称{method.Name}");
            }

            var json = _jsonParser.ToJson(arg1);

            return Enqueue(sAttr.ServiceId, mAttr.MessageId, json);
        }

        public async Task<RpcResult> EnqueueEx(int serviceId, ushort messageId, string executeJson, string routeKey)
        {
            RpcResult result = new RpcResult();

            try
            {
                QueueTaskItem item = new QueueTaskItem(serviceId, messageId, executeJson, routeKey);
                await _messageBus.PublishAsync(item);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "enqueueex task fail:" + e.Message);
                result.Code = RpcErrorCodes.CODE_INTERNAL_ERROR;
            }

            return result;
        }

        public async Task<RpcResult> EnqueueDelayEx(int serviceId, ushort messageId, string executeJson, TimeSpan delayTime, string routeKey)
        {
            var delayTask = new DelayTask
            {
                ServiceId = serviceId,
                MessageId = messageId,
                ExecuteJson = executeJson,
                ExecuteTime = DateTime.Now.Add(delayTime),
                TaskCode = null,
                Status = 1,
                CheckCode = null,
                Description = null,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                RouteKey = routeKey
            };
            return await EnqueueDelay(delayTask);
        }

        public async Task<RpcResult> EnqueueDelay(int serviceId, ushort messageId, string executeJson, TimeSpan delayTime, string taskCode = null,
            string checkCode = null, string description = null)
        {
            var delayTask = new DelayTask
            {
                ServiceId = serviceId,
                MessageId = messageId,
                ExecuteJson = executeJson,
                ExecuteTime = DateTime.Now.Add(delayTime),
                TaskCode = taskCode,
                Status = 1,
                CheckCode = checkCode,
                Description = description,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
            };
            return await EnqueueDelay(delayTask);
        }

        public async Task<RpcResult> EnqueueDelay(IDelayTask delayTask)
        {
            RpcResult result = new RpcResult();
            try
            {
                var delayTime = delayTask.ExecuteTime - DateTime.Now;
                if (_options.DbPullInterval >= delayTime.TotalSeconds) //短时间的延迟任务
                {
                    delayTask.Status = 8;
                }
                var ret = await _dbStorage.AddDelayTask(delayTask);

                if (ret <= 0)
                {
                    result.Code = RpcErrorCodes.CODE_INTERNAL_ERROR;
                    _logger.LogError("持久化延迟任务到DB出错");
                    return result;
                }

                delayTask.TaskId = ret;

                if (_options.DbPullInterval >= delayTime.TotalSeconds) //短时间的延迟任务
                {
                    await _redisStorage.Push(new List<IDelayTask> { delayTask });
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e, "enqueue task fail:" + e.Message);
                result.Code = RpcErrorCodes.CODE_INTERNAL_ERROR;
            }

            return result;
        }

        public Task<RpcResult> EnqueueDelay<T>(Func<T, Task> delayFunc, T arg1, TimeSpan delayTime, string taskCode = null, string checkCode = null,
            string description = null) where T : class
        {
            var interfaces = delayFunc.Method.DeclaringType.FindInterfaces((t, o) => t.GetCustomAttribute<RpcServiceAttribute>() != null, null);
            if (interfaces.Length != 1)
            {
                throw new NotSupportedException("不支持的服务调用,服务必须是具备RpcServiceAttribute的接口实现类");
            }

            var serviceType = interfaces[0];
            var method = serviceType.GetMethod(delayFunc.Method.Name);
            var sAttr = serviceType.GetCustomAttribute<RpcServiceAttribute>();
            var mAttr = method.GetCustomAttribute<RpcMethodAttribute>();

            if (sAttr == null)
            {
                throw new NotSupportedException($"不支持的服务调用，服务类型{serviceType.Name}");
            }

            if (mAttr == null)
            {
                throw new NotSupportedException($"不支持的方法调用，方法名称{method.Name}");
            }

            var json = _jsonParser.ToJson(arg1);

            return EnqueueDelay(sAttr.ServiceId, mAttr.MessageId, json, delayTime, taskCode, checkCode, description);
        }

        public async Task<RpcResult> CancelDelay(string checkCode)
        {
            var res = new RpcResult();
            var ret = await _dbStorage.CancelTask(checkCode);
            if (ret > 0)
            {
                await _redisStorage.CancelTask(checkCode);
            }

            return res;
        }
    }
}
