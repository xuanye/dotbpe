using DotBPE.Rpc;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Logging;
using System;
using System.Threading.Tasks;

namespace DotBPE.Protocol.Amp
{
    public abstract class ServiceActor : IServiceActor<AmpMessage>
    {
        static readonly ILogger Logger = DotBPE.Rpc.Environment.Logger.ForType<ServiceActor>();

        protected abstract int ServiceId { get; }

        public string Id {
            get
            {
                return $"{this.ServiceId}$0";
            }
        }


        public virtual async Task ReceiveAsync(IRpcContext<AmpMessage> context, AmpMessage message)
        {
            try
            {
                var response = await ProcessAsync(message);
                response.Sequence = message.Sequence; //通讯请求序列
                await context.SendAsync(response);
            }
            catch(Exception ex)
            {
                Logger.Error(ex,"recieve message occ error:"+ex.Message);
                await SendErrorResponseAsync(context,message);
            }
        }
        /// <summary>
        /// 发送服务端意外错误的消息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="reqMessage"></param>
        /// <returns></returns>
        private Task SendErrorResponseAsync(IRpcContext<AmpMessage> context, AmpMessage reqMessage){

            try
            {
                var rsp = AmpMessage.CreateResponseMessage(reqMessage.ServiceId,reqMessage.MessageId);
                rsp.InvokeMessageType = InvokeMessageType.Response;
                rsp.Sequence = reqMessage.Sequence;
                rsp.Code = ErrorCodes.CODE_INTERNAL_ERROR; //内部错误
                return context.SendAsync(rsp);
            }
            catch(Exception ex)
            {
                Logger.Error(ex,"send error response fail:"+ex.Message);
                return Rpc.Utils.TaskUtils.CompletedTask;
            }

        }

        public abstract Task<AmpMessage> ProcessAsync(AmpMessage req);

        protected Task<AmpMessage> ProcessNotFoundAsync(AmpMessage req)
        {
            var response = AmpMessage.CreateResponseMessage(req.ServiceId, req.MessageId);
            response.Sequence = req.Sequence;
            response.InvokeMessageType = InvokeMessageType.Response;
            response.Code = ErrorCodes.CODE_SERVICE_NOT_FOUND; 

            Logger.Warning("recieve message serviceId={0},messageId={1},Actor NotFound",req.ServiceId,req.MessageId);
            return Task.FromResult(response);
        }

    }
}
