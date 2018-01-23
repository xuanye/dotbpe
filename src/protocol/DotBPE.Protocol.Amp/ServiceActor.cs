using DotBPE.Rpc;
using DotBPE.Rpc.Codes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading.Tasks;

namespace DotBPE.Protocol.Amp
{
    public abstract class ServiceActor : IServiceActor<AmpMessage>
    {
        private ILogger _Logger;
        
       
        protected abstract int ServiceId { get; }

        protected ILogger Logger
        {
            get
            {
                if (this._Logger == null)
                {
                    if (Rpc.Environment.LoggerFactory != null)
                    {
                        this._Logger = Rpc.Environment.LoggerFactory.CreateLogger<ServiceActor>();
                    }
                    else
                    {
                        this._Logger = NullLogger.Instance;
                    }

                }
                return this._Logger;
            }
        }

        public string Id
        {
            get
            {
                return $"{this.ServiceId}$0";
            }
        }

        public virtual async Task ReceiveAsync(IRpcContext<AmpMessage> context, AmpMessage message)
        {
            try
            {
                Logger.LogDebug("recieve message,Id={0}", message.Id);
                //TODO:这里可以写CSOS_AUDIT日志
                var response = await ProcessAsync(message);
                response.Sequence = message.Sequence; //通讯请求序列
                await context.SendAsync(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "recieve message occ error:" + ex.Message);
                await SendErrorResponseAsync(context, message);
            }
        }

        /// <summary>
        /// 发送服务端意外错误的消息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="reqMessage"></param>
        /// <returns></returns>
        private Task SendErrorResponseAsync(IRpcContext<AmpMessage> context, AmpMessage reqMessage)
        {
            try
            {
                var rsp = AmpMessage.CreateResponseMessage(reqMessage.ServiceId, reqMessage.MessageId);
                rsp.InvokeMessageType = InvokeMessageType.Response;
                rsp.Sequence = reqMessage.Sequence;
                rsp.Code = ErrorCodes.CODE_INTERNAL_ERROR; //内部错误
                return context.SendAsync(rsp);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "send error response fail:" + ex.Message);
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

            Logger.LogWarning("recieve message serviceId={0},messageId={1},Actor NotFound", req.ServiceId, req.MessageId);
            return Task.FromResult(response);
        }
    }
}
