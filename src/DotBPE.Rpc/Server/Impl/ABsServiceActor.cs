using DotBPE.Rpc.Protocol;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Peach;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Environment = DotBPE.Rpc.Internal.Environment;
namespace DotBPE.Rpc.Server
{
    public abstract class AbsServiceActor : IServiceActor<AmpMessage>
    {
        private ILogger _logger;
        protected ILogger Logger
        {
            get
            {
                if (_logger == null)
                {
                    if (Environment.LoggerFactory != null)
                    {
                        _logger = Environment.LoggerFactory.CreateLogger<AbsServiceActor>();
                    }
                    else
                    {
                        _logger = NullLogger.Instance;
                    }
                }
                return _logger;
            }
        }

        public abstract string Id { get; }


        /// <summary>
        /// 处理接收消息
        /// TODO: 处理审计日志的问题
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task ReceiveAsync(ISocketContext<AmpMessage> context, AmpMessage message)
        {
            AmpMessage rsp;
            try
            {

                Logger.LogDebug("Receive message,Id={0}", message.Id);
                rsp = await ProcessAsync(message);
                rsp.Sequence = message.Sequence; //通讯请求序列

                await context.SendAsync(rsp);

                //Logger.LogError("send message,Id={0}", message.Id);
            }
            catch (ClosedChannelException closedEx)
            {
                Logger.LogError(closedEx, "Receive message occ error,channel closed,{messageId}", message.Id);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Receive message occ error ,messageId={messageId}", message.Id);
                rsp = await SendErrorResponseAsync(context, message);

            }
        }

        public abstract Task<AmpMessage> ProcessAsync(AmpMessage req);
        public abstract object Invoke(ushort messageId,params object[] args);
        /// <summary>
        /// 发送服务端意外错误的消息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="reqMessage"></param>
        /// <returns></returns>
        private async Task<AmpMessage> SendErrorResponseAsync(ISocketContext<AmpMessage> context, AmpMessage reqMessage)
        {
            var rsp = AmpMessage.CreateResponseMessage(reqMessage.ServiceId, reqMessage.MessageId);
            rsp.InvokeMessageType = InvokeMessageType.Response;
            rsp.Sequence = reqMessage.Sequence;
            rsp.Code = RpcErrorCodes.CODE_INTERNAL_ERROR; //内部错误
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
    }
}
