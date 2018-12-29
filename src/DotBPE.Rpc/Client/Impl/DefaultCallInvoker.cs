using DotBPE.Rpc.Protocol;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    public class DefaultCallInvoker : ICallInvoker<AmpMessage>
    {
        private readonly IClientMessageHandler<AmpMessage> _handler;
        private readonly IRpcClient<AmpMessage> _rpcClient;
        private readonly ILogger<DefaultCallInvoker> _logger;


        private readonly ConcurrentDictionary<string, TaskCompletionSource<AmpMessage>>
            _resultDictionary =new ConcurrentDictionary<string, TaskCompletionSource<AmpMessage>>();

        private static int INVOKER_SEQ = 0;
        private static object lockObj = new object();

        public DefaultCallInvoker(
            IClientMessageHandler<AmpMessage> handler,
            IRpcClient<AmpMessage> rpcClient,
            ILogger<DefaultCallInvoker> logger
        )
        {
            _handler = handler;
            _rpcClient = rpcClient;
            _logger = logger;


            _handler.OnReceived += _handler_OnReceived;

        }

        /// <summary>
        /// 调用返回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _handler_OnReceived(object sender, AmpMessage message)
        {
            if(message.InvokeMessageType == InvokeMessageType.Response)
            {
                //处理消息
                MessageRecieved(message);
            }
        }

        /// <summary>
        /// TODO:记录审计日志
        /// </summary>
        /// <param name="request"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public async Task<AmpMessage> AsyncCall(AmpMessage request, int timeOut = 3000)
        {
            AutoSetSequence(request);
            _logger.LogDebug("new request id={0}", request.Id);

            if(request.InvokeMessageType == InvokeMessageType.InvokeWithoutResponse)
            {
                await SendAsync(request);
                return AmpMessage.CreateRequestMessage(request.ServiceId,request.MessageId);
            }

            var cts = new CancellationTokenSource(timeOut);
            //timeout callback
            using (cts.Token.Register(() => TimeOutCallBack(request.Id), useSynchronizationContext: false))
            {
                //register callback
                var callbackTask = RegisterResultCallbackAsync(request.Id, timeOut);

                //async call
                await SendAsync(request);

                //get return message
                var rsp = await callbackTask;
             
                return rsp;
            }
        }



        public AmpMessage BlockingCall(AmpMessage request, int timeOut = 3000)
        {
            return this.AsyncCall(request, timeOut).Result;
        }


        private async Task<bool> SendAsync(AmpMessage request)
        {
            bool success = false;
            try
            {
                //发送
                await this._rpcClient.SendAsync(request);
                success = true;
            }
            catch (Exception exception)
            {
                success = false;
                ErrorCallBack(request.Id);
                _logger.LogError(exception, "error occor:");
            }
            return success;
        }

        private void MessageRecieved(AmpMessage message)
        {
         
            if (message.ServiceId == 0 && message.MessageId == 0)
            {                
                _logger.LogTrace("recieved heart beat");
                return;
            }

            TaskCompletionSource<AmpMessage> task;
            if (message.Code != 0)
            {
                _logger.LogDebug("server response error msg ,type={0}", message.InvokeMessageType);              
                if (_resultDictionary.TryRemove(message.Id, out task))
                {
                    task.TrySetResult(message);
                    _logger.LogDebug("message {0},set result success,message.code ={1}", message.Id, message.Code);                   
                }
                else
                {                  
                    _logger.LogError(string.Format("server response error msg ,id={0},code={1},", message.Id, message.Code));
                }
            }
            else 
            {
                _logger.LogDebug($"receive message, id:{message.Id}");             
                if (_resultDictionary.TryRemove(message.Id, out task))
                {
                    task.TrySetResult(message);
                    _logger.LogDebug("message {0},set result success,message.code ={1}", message.Id, message.Code);                  
                }
              
            }
           
        }
              

        private void TimeOutCallBack(string id)
        {
            TaskCompletionSource<AmpMessage> task;
            if (_resultDictionary.TryRemove(id, out task))
            {
                var message = AmpMessage.CreateResponseMessage(id);
                message.Code = RpcErrorCodes.CODE_TIMEOUT;
                if (!task.TrySetResult(message))
                {
                    _logger.LogWarning("set timeout result fail,maybe task is completed,message {0}", id);
                }
                _logger.LogWarning("message {0}, timeout", id);              
            }
        }

        private void ErrorCallBack(string id)
        {
            if (!_resultDictionary.ContainsKey(id))
            {
                return;
            }
            TaskCompletionSource<AmpMessage> task;
            if (_resultDictionary.TryRemove(id, out task))
            {
                var message = AmpMessage.CreateResponseMessage(id);
                message.Code = RpcErrorCodes.CODE_INTERNAL_ERROR;
                if (!task.TrySetResult(message))
                {
                    _logger.LogWarning("set error result fail,maybe task is completed");
                }
                _logger.LogWarning("message {0}, error", id);             
            }
        }

        private Task<AmpMessage> RegisterResultCallbackAsync(string id, int timeOut)
        {
            var tcs = new TaskCompletionSource<AmpMessage>();

            _resultDictionary.TryAdd(id, tcs);
            return tcs.Task;
        }

        private void AutoSetSequence(AmpMessage request)
        {
            int id = Interlocked.Increment(ref INVOKER_SEQ);
            request.Sequence = id;
        }
    }
}
