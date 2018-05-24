using DotBPE.Rpc;
using DotNetty.Transport.Channels;
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
            using (var audit = new RequestAuditLogger())
            {
                AmpMessage rsp;
                try
                {
                    audit.PushRequest(message);
                    Logger.LogDebug("recieve message,Id={0}", message.Id);
                    rsp = await ProcessAsync(message);
                    rsp.Sequence = message.Sequence; //通讯请求序列

                    audit.PushResponse(rsp);
                    audit.PushContext(context);

                    await context.SendAsync(rsp);

                    //Logger.LogError("send message,Id={0}", message.Id);
                }
                catch (ClosedChannelException closedEx)
                {
                    Logger.LogError(closedEx, "recieve message occ error,channel closed,{messageId}", message.Id);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "recieve message occ error");
                    rsp = await SendErrorResponseAsync(context, message);
                    audit.PushResponse(rsp);
                }
            }
        }

        /// <summary>
        /// 发送服务端意外错误的消息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="reqMessage"></param>
        /// <returns></returns>
        private async Task<AmpMessage> SendErrorResponseAsync(IRpcContext<AmpMessage> context, AmpMessage reqMessage)
        {
            var rsp = AmpMessage.CreateResponseMessage(reqMessage.ServiceId, reqMessage.MessageId);
            rsp.InvokeMessageType = InvokeMessageType.Response;
            rsp.Sequence = reqMessage.Sequence;
            rsp.Code = ErrorCodes.CODE_INTERNAL_ERROR; //内部错误
            try
            {
                await context.SendAsync(rsp);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "send error response fail:" + ex.Message);
            }

            return rsp;
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
