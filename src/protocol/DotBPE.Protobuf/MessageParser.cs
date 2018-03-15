using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using System;
using System.Text.RegularExpressions;

namespace DotBPE.Protobuf
{
    public class MessageParser : IMessageParser<AmpMessage>
    {
        private static readonly JsonFormatter AmpJsonFormatter = new JsonFormatter(new JsonFormatter.Settings(true).WithFormatEnumsAsIntegers(true));

        private readonly ILogger<MessageParser> _logger;
        private readonly IProtobufObjectFactory _factory;

        public MessageParser(IProtobufObjectFactory factory, ILogger<MessageParser> logger)
        {
            _logger = logger;
            _factory = factory;
        }

        public virtual string ToJson(AmpMessage message)
        {
            if (message.Code == 0)
            {
                var return_code = 0;
                var return_message = "";
                string ret = "";
                if (message != null)
                {
                    var rspTemp = _factory.GetResponseTemplate(message.ServiceId, message.MessageId);
                    if (rspTemp == null)
                    {
                        return ret;
                    }

                    if (message.Data != null)
                    {
                        rspTemp.MergeFrom(message.Data);
                    }

                    //提取return_message
                    var field_msg = rspTemp.Descriptor.FindFieldByName("return_message");
                    if (field_msg != null)
                    {
                        var retObjV = field_msg.Accessor.GetValue(rspTemp);
                        if (retObjV != null)
                        {
                            return_message = retObjV.ToString();
                        }
                    }

                    ret = AmpJsonFormatter.Format(rspTemp);
                }

                return string.Concat("{\"return_code\":",return_code.ToString(), ",\"return_message\":\"",return_message, "\",\"data\":",ClearMetaField(ret), "}");
            }
            else
            {
                return string.Concat("{\"return_code\":" , message.Code , ",\"return_message\":\"\"}");
            }
        }

        public virtual AmpMessage ToMessage(RequestData reqData)
        {
            ushort serviceId = (ushort)reqData.ServiceId;
            ushort messageId = (ushort)reqData.MessageId;
            AmpMessage message = AmpMessage.CreateRequestMessage(serviceId, messageId);

            IMessage reqTemp = _factory.GetRequestTemplate(serviceId, messageId);
            if (reqTemp == null)
            {
                this._logger.LogError("serviceId={0},messageId={1}的消息模板不存在", serviceId, messageId);
                return null;
            }

            try
            {
                var descriptor = reqTemp.Descriptor;
                if (!string.IsNullOrEmpty(reqData.RawBody))
                {
                    reqTemp = descriptor.Parser.ParseJson(reqData.RawBody);
                }

                if ( reqData.Data !=null && reqData.Data.Count > 0)
                {
                    foreach (var field in descriptor.Fields.InDeclarationOrder())
                    {
                        if (reqData.Data.ContainsKey(field.Name))
                        {
                            ProtobufHelper.SetValue(field.Accessor,reqTemp,reqData.Data[field.Name]);
                        }
                        else if (reqData.Data.ContainsKey(field.JsonName))
                        {
                            ProtobufHelper.SetValue(field.Accessor,reqTemp,reqData.Data[field.JsonName]);
                        }
                    }
                }
                message.Data = reqTemp.ToByteArray();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "从请求中解析数据错误");
                message = null;
            }

            return message;
        }

        protected virtual string ClearMetaField(string rspJson)
        {
            rspJson = Regex.Replace(rspJson, " \"returnMessage\"[\\040]*:[\\040]*\"[^\"]*\",", "");
            rspJson = Regex.Replace(rspJson, " ,\"bpeSessionId\"[\\040]*:[\\040]*\"[\\w]*\"", "");
            return rspJson;
        }
    }
}
