using System;
using System.Threading.Tasks;
using DotBPE.Rpc;
using System.Collections.Concurrent;
using DotBPE.Rpc.Logging;
using DotBPE.Rpc.Exceptions;

namespace DotBPE.Protocol.Amp
{
    public class AmpCallInvoker : CallInvoker<AmpMessage>
    {
        static readonly ILogger Logger = DotBPE.Rpc.Environment.Logger.ForType<AmpMessage>();

        private readonly ConcurrentDictionary<string, TaskCompletionSource<AmpMessage>> _resultDictionary =
new ConcurrentDictionary<string, TaskCompletionSource<AmpMessage>>();

        private int sendSequence = 0 ;
        private static object lockObj = new object();
        public AmpCallInvoker(IRpcClient<AmpMessage> rpcClient) : base(rpcClient)
        {
        }
     

        public async override Task<AmpMessage> AsyncCall(AmpMessage request)
        {
            try
            {
                AutoSetSequence(request);
                var callbackTask = RegisterResultCallbackAsync(request.Id);               
                try
                {

                    //发送
                    await base.RpcClient.SendAsync(request);
                }
                catch (Exception exception)
                {
                    throw new RpcCommunicationException ("与服务端通讯时发生了异常。", exception);
                }

                Logger.Debug("消息发送成功。");

                return await callbackTask;
            }
            catch (Exception exception)
            {             
                Logger.Error("消息发送失败。", exception);
                throw;
            }
        }

       
        public override AmpMessage BlockingCall(AmpMessage request)
        {
            return this.AsyncCall(request).Result;           
        }

        protected override void MessageRecieved(object sender, MessageRecievedEventArgs<AmpMessage> e)
        {
            if(e.Message == null)
            {
                throw new RpcBizException("收到了空消息");
            }

            if(e.Message.InvokeMessageType == Rpc.Codes.InvokeMessageType.Response) //只处理回复消息
            {                
                Logger.Info($"接收到消息:{e.Message.Id}");
                var message = e.Message;
                TaskCompletionSource<AmpMessage> task;
                if (_resultDictionary.TryGetValue(message.Id, out task))
                {
                    task.SetResult(message);

                    // 移除字典
                    try
                    {
                        _resultDictionary.TryRemove(message.Id, out var _);
                    }
                    catch
                    {

                    }
                }
            }
        }


        private async Task<AmpMessage> RegisterResultCallbackAsync(string id)
        {

            Logger.Debug($"准备获取Id为：{id}的响应内容。");

            var task = new TaskCompletionSource<AmpMessage>();
            _resultDictionary.TryAdd(id, task);
            try
            {
                var result = await task.Task;
                return result;
            }
            finally
            {
                //删除回调任务              
                _resultDictionary.TryRemove(id, out var _);
            }
        }

        private void AutoSetSequence(AmpMessage request)
        {
            lock (lockObj)
            {
                request.Sequence = this.sendSequence++;
            }           
        }
    }
}