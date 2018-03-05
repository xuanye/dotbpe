using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace DotBPE.Protocol.Amp
{
    public class AmpCallInvoker : AbstractCallInvoker<AmpMessage>
    {
        private ILogger _Logger;

        private readonly ConcurrentDictionary<string, TaskCompletionSource<AmpMessage>> _resultDictionary =
new ConcurrentDictionary<string, TaskCompletionSource<AmpMessage>>();

        private static int INVOKER_SEQ = 0;
        private static object lockObj = new object();

        public AmpCallInvoker(IRpcClient<AmpMessage> client) : this(client,null)
        {

        }

        public AmpCallInvoker(IRpcClient<AmpMessage> client,ILogger<AmpCallInvoker> logger) : base(client)
        {
           if(logger == null)
           {
               _Logger = NullLogger.Instance;
           }
           else
           {
               _Logger = logger;
           }
        }

        protected ILogger Logger
        {
            get
            {
                return this._Logger;
            }
        }

        /// <summary>
        /// 异步调用，可以设置调用超时
        /// </summary>
        /// <param name="request">请求的消息</param>
        /// <param name="timeOut">设置超时，默认5000 ，单位为毫秒</param>
        /// <returns>返回消息</returns>
        /// <exception cref="RpcCommunicationException">rpc communication error</exception>
        public async override Task<AmpMessage> AsyncCall(AmpMessage request, int timeOut = 5000)
        {
            using (CACAuditLogger auditLogger = new CACAuditLogger())
            {

                AutoSetSequence(request);

                auditLogger.PushRequest(request); //记录请求参数

                Logger.LogDebug("new request id={0}", request.Id);
                var callbackTask = RegisterResultCallbackAsync(request.Id, timeOut);

                try
                {
                    //发送
                    await base.RpcClient.SendAsync(request);
                }
                catch (Exception exception)
                {
                    ErrorCallBack(request.Id);
                    Logger.LogError(exception, "error occor:");
                }

                var rsp = await callbackTask;
                auditLogger.PushResponse(rsp); //记录响应
                return rsp;
            }
                
        }

        public override AmpMessage BlockingCall(AmpMessage request, int timeOut = 5000)
        {
            return this.AsyncCall(request, timeOut).Result;
        }
        

        protected override void MessageRecieved(object sender, MessageRecievedEventArgs<AmpMessage> e)
        {
            if (e.Message == null)
            {
                throw new RpcBizException("empty message");
            }
            if (e.Message.ServiceId == 0 && e.Message.MessageId == 0)
            {
                //心跳消息
                Logger.LogDebug("heart beat");
                return;
            }

            if (e.Message.InvokeMessageType == InvokeMessageType.Response)
            {
                if (e.Message.Code != 0)
                {
                    Logger.LogError("server response error msg ,type{0}", e.Message.InvokeMessageType);
                    var message = e.Message;
                    TaskCompletionSource<AmpMessage> task;
                    if (_resultDictionary.ContainsKey(message.Id)
                        && _resultDictionary.TryGetValue(message.Id, out task))
                    {
                        task.SetResult(e.Message);
                        Logger.LogError(string.Format("server response error msg ,code={0}", e.Message.Code));
                        // 移除字典
                        RemoveResultCallback(message.Id);
                    }
                    else
                    {
                        //TODO:详细的错误日志信息
                        Logger.LogError(string.Format("server response error msg ,code={0},", e.Message.Code));
                    }
                }
                else //正常返回
                {
                    Logger.LogDebug($"receive message, id:{e.Message.Id}");
                    var message = e.Message;
                    TaskCompletionSource<AmpMessage> task;
                    if (_resultDictionary.ContainsKey(message.Id)
                        && _resultDictionary.TryGetValue(message.Id, out task))
                    {
                        task.TrySetResult(message);
                        Logger.LogDebug("message {0},set result success", message.Id);
                        // 移除字典
                        RemoveResultCallback(message.Id);
                    }
                    //消息通传到每个AmpCallInvoker，如果这里没有，那就再其他示例中
                }
            }
        }

        private void RemoveResultCallback(string id)
        {
            var removed = _resultDictionary.TryRemove(id, out var _);
            Logger.LogDebug("message {0},remove from queue {1}", id, removed ? "success" : "fail");
        }

        private void TimeOutCallBack(string id)
        {
            TaskCompletionSource<AmpMessage> task;
            if (_resultDictionary.TryGetValue(id, out task))
            {
                var message = AmpMessage.CreateResponseMessage(id);
                message.Code = ErrorCodes.CODE_TIMEOUT;
                if (!task.TrySetResult(message))
                {
                    Logger.LogWarning("set timeout result fail,maybe task is completed");
                }
               
                Logger.LogWarning("message {0}, timeout", id);
                //task.TrySetException(new RpcCommunicationException("operation timeout"));
                // 移除字典
                RemoveResultCallback(id);
            }
        }

        private void ErrorCallBack(string id)
        {
            TaskCompletionSource<AmpMessage> task;
            if (_resultDictionary.TryGetValue(id, out task))
            {
                var message = AmpMessage.CreateResponseMessage(id);
                message.Code = ErrorCodes.CODE_INTERNAL_ERROR;
                if (!task.TrySetResult(message))
                {
                    Logger.LogWarning("set error result fail,maybe task is completed");
                }

                Logger.LogWarning("message {0}, error", id);
                //task.TrySetException(new RpcCommunicationException("operation timeout"));
                // 移除字典
                RemoveResultCallback(id);
            }
        }
        private Task<AmpMessage> RegisterResultCallbackAsync(string id, int timeOut)
        {
            var tcs = new TaskCompletionSource<AmpMessage>();

            _resultDictionary.TryAdd(id, tcs);
            var task = tcs.Task;
            Task.Factory.StartNew(() =>
            {
                if (Task.WhenAny(task, Task.Delay(timeOut)).Result != task)
                {
                    // timeout logic
                    TimeOutCallBack(id);
                }
            });

            return task;
        }

        private void AutoSetSequence(AmpMessage request)
        {
            
            int id = Interlocked.Increment(ref INVOKER_SEQ);
            request.Sequence = id;
        }
    }
}
