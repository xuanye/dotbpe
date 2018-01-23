using DotBPE.Rpc;
using DotBPE.Rpc.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace DotBPE.Protocol.Amp
{
    public class AmpCallInvoker : CallInvoker<AmpMessage>
    {
        private ILogger _Logger;

        private readonly ConcurrentDictionary<string, TaskCompletionSource<AmpMessage>> _resultDictionary =
new ConcurrentDictionary<string, TaskCompletionSource<AmpMessage>>();

        private int sendSequence = 0;
        private static object lockObj = new object();

        public AmpCallInvoker(string serverAddress, int multiplexCount = 1) : this(AmpClient.Create(serverAddress, multiplexCount))
        {
        }

        public AmpCallInvoker(IRpcClient<AmpMessage> client) : base(client)
        {
           
        }

        protected ILogger Logger
        {
            get
            {
                if (this._Logger == null )
                {
                    if(Rpc.Environment.LoggerFactory != null)
                    {
                        this._Logger = Rpc.Environment.LoggerFactory.CreateLogger<AmpCallInvoker>();
                    }
                    else
                    {
                        this._Logger = NullLogger.Instance;
                    }
                   
                }
                return this._Logger;
            }
        }

        public async override Task<AmpMessage> AsyncCall(AmpMessage request, int timeOut = 5000)
        {
            try
            {
                AutoSetSequence(request);
                Logger.LogDebug("new request id={0}", request.Id);
                var callbackTask = RegisterResultCallbackAsync(request.Id, timeOut);

                try
                {
                    //发送
                    await base.RpcClient.SendAsync(request);
                }
                catch (Exception exception)
                {
                    RemoveResultCallback(request.Id);
                    throw new RpcCommunicationException("rpc communication error", exception);
                }

                return await callbackTask;
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, "error occor:");
                throw exception;
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
                Logger.LogInformation("heart beat message Id{0}", e.Message.Id);
                return;
            }

            if (e.Message.InvokeMessageType == Rpc.Codes.InvokeMessageType.Response)
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
                    else
                    {
                        //TODO:详细的错误日志信息
                        Logger.LogError("server response,but no handler,Id={0}"
                            ,e.Message.Id);
                    }
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
                task.SetResult(message);
                Logger.LogWarning("message {0}, timeout", id);
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
            
            int id = Interlocked.Increment(ref this.sendSequence);
            request.Sequence = id;
        }
    }
}
