using DotBPE.Rpc;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Logging;
using System;
using System.Threading.Tasks;

namespace DotBPE.Protocol.Amp
{
    public abstract class ServiceActorBase : IServiceActor<AmpMessage>
    {
        static readonly ILogger Logger = DotBPE.Rpc.Environment.Logger.ForType<ServiceActorBase>();
        public abstract string Id {get;}


        public virtual async Task ReceiveAsync(IRpcContext<AmpMessage> context, AmpMessage message)
        {
            try
            {
                var response = await ProcessAsync(message);
            }
            catch(Exception ex)
            {
                Logger.Error(ex,"recieve message occ error:"+ex.Message);
                await SendErrorResponseTask(context,message);
            }
        }
        /// <summary>
        /// 发送服务端意外错误的消息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="reqMessage"></param>
        /// <returns></returns>
        private Task SendErrorResponseTask(IRpcContext<AmpMessage> context, AmpMessage reqMessage){

            try
            {
                var rsp = AmpMessage.CreateResponseMessage(reqMessage.ServiceId,reqMessage.MessageId);
                rsp.InvokeMessageType = InvokeMessageType.ERROR;
                rsp.Sequence = reqMessage.Sequence;
                return context.SendAsync(rsp);
            }
            catch(Exception ex)
            {
                Logger.Error(ex,"send error response fail:"+ex.Message);
                return Rpc.Utils.TaskUtils.CompletedTask;
            }

        }

        public abstract Task<AmpMessage> ProcessAsync(AmpMessage reqMessage);

        protected Task<AmpMessage> ProcessNotFoundAsync(AmpMessage req)
        {
            var response = AmpMessage.CreateResponseMessage(req.ServiceId, req.MessageId);
            response.Sequence = req.Sequence;
            response.InvokeMessageType = InvokeMessageType.NotFound;
            Logger.Error("recieve message serviceId={0},messageId={1},Length ={2},Actor NotFound",req.ServiceId,req.MessageId,req.Length);
            return Task.FromResult(response);
        }

    }
}
