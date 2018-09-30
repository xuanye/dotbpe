
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using System;

namespace DotBPE.Protobuf
{
    /// <summary>
    /// 用于服务调用时的消息转换
    /// </summary>
    /// <seealso cref="DotBPE.Rpc.IMessageParser{DotBPE.Protocol.Amp.AmpMessage}" />
    public class MessageParser : IMessageParser<AmpMessage>
    {
        private static readonly JsonFormatter AmpJsonFormatter = new JsonFormatter(new JsonFormatter.Settings(false).WithFormatEnumsAsIntegers(true));

        private readonly ILogger<MessageParser> _logger;
        private readonly IProtobufDescriptorFactory _factory;

        public MessageParser(IProtobufDescriptorFactory factory, ILogger<MessageParser> logger)
        {
            _logger = logger;
            _factory = factory;
        }

        /// <summary>
        /// 将Amp消息转换成JSON格式
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public virtual string ToJson(AmpMessage message)
        {
            var return_message = "";
            string ret = "";
            if (message != null)
            {
                IMessage rspTemp = null;
                var rspDescriptor = _factory.GetResponseDescriptor(message.ServiceId, message.MessageId);
                if (rspDescriptor == null)
                {
                    return ret;
                }

                if (message.Data != null)
                {
                    rspTemp = rspDescriptor.Parser.ParseFrom(message.Data);
                }

                if (rspTemp == null)
                {
                    return string.Concat("{\"returnCode\":", message.Code, ",\"returnMessage\":\"\"}");
                }

                //提取return_message
                var field_msg = rspDescriptor.FindFieldByName("return_message");
                if (field_msg != null)
                {
                    var retObjV = field_msg.Accessor.GetValue(rspTemp);
                    if (retObjV != null)
                    {
                        return_message = retObjV.ToString();
                    }
                    field_msg.Accessor.SetValue(rspTemp, ""); //设置内层的returnMessage为空
                }
                ret = AmpJsonFormatter.Format(rspTemp);

                rspTemp = null;

                return string.Concat("{\"returnCode\":", message.Code, ",\"returnMessage\":\"", return_message, "\",\"data\":", ret, "}");
            }
            else
            {
                return string.Concat("{\"returnCode\":", ErrorCodes.CODE_INTERNAL_ERROR, ",\"returnMessage\":\"内部错误\"}");
            }
        }

        /// <summary>
        /// 将组织好的Http请求消息，转换成AmpMessage
        /// </summary>
        /// <param name="reqData">The req data.</param>
        /// <returns></returns>
        public virtual AmpMessage ToMessage(RequestData reqData)
        {
            ushort serviceId = (ushort)reqData.ServiceId;
            ushort messageId = (ushort)reqData.MessageId;
            AmpMessage message = AmpMessage.CreateRequestMessage(serviceId, messageId);

            IMessage reqTemp = null;
            var reqDescriptor = _factory.GetRequestDescriptor(serviceId, messageId);
            if (reqDescriptor == null)
            {
                this._logger.LogError("serviceId={serviceId},messageId={messageId}的请求消息模板不存在", serviceId, messageId);
                return null;
            }

            try
            {
                if (!string.IsNullOrEmpty(reqData.RawBody))
                {
                    reqTemp = reqDescriptor.Parser.ParseJson(reqData.RawBody);
                }
                if (reqTemp == null)
                {
                    reqTemp = reqDescriptor.Parser.ParseJson("{}");//空对象
                }
                if (reqData.Data != null && reqData.Data.Count > 0)
                {
                    foreach (var field in reqDescriptor.Fields.InDeclarationOrder())
                    {
                        if (reqData.Data.ContainsKey(field.Name))
                        {
                            ProtobufHelper.SetValue(field.Accessor, reqTemp, reqData.Data[field.Name]);
                        }
                        else if (reqData.Data.ContainsKey(field.JsonName))
                        {
                            ProtobufHelper.SetValue(field.Accessor, reqTemp, reqData.Data[field.JsonName]);
                        }
                    }
                }
                message.Data = reqTemp.ToByteArray();

                reqTemp = null;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "从请求中解析数据错误");
                message = null;
            }

            return message;
        }
    }
}
