using System;
using System.Collections.Generic;
using System.Text;
using DotBPE.Plugin.AspNetGateway;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using DotBPE.Rpc.Logging;
using Google.Protobuf;
using Microsoft.Extensions.Options;

namespace GatewayForAspNet
{
    /// <summary>
    /// 转发服务，需要根据自己的协议 将HTTP请求的数据转码成对应的协议二进制
    /// 并将协议二进制数据转换成对应的 Http响应数据（一般是json） 
    /// </summary>
    public class ForwardService : AbstractForwardService<AmpMessage>
    {

        static readonly JsonFormatter AmpJsonFormatter = new JsonFormatter(new JsonFormatter.Settings(true));

        static ILogger Logger = DotBPE.Rpc.Environment.Logger.ForType<ForwardService>();


        public ForwardService(IRpcClient<AmpMessage> rpcClient,
           IOptionsSnapshot<HttpRouterOption> optionsAccessor) : base(rpcClient, optionsAccessor)
        {
        }

        protected override AmpMessage EncodeRequest(RequestData reqData)
        {
            ushort serviceId = (ushort)reqData.ServiceId;
            ushort messageId = (ushort)reqData.MessageId;
            AmpMessage message = AmpMessage.CreateRequestMessage(serviceId, messageId);


            IMessage reqTemp = ProtobufObjectFactory.GetRequestTemplate(serviceId, messageId);
            if (reqTemp == null)
            {
                return null;
            }

            try
            {
                var descriptor = reqTemp.Descriptor;
                if (!string.IsNullOrEmpty(reqData.RawBody))
                {
                    reqTemp = descriptor.Parser.ParseJson(reqData.RawBody);
                }

                if (reqData.Data.Count > 0)
                {
                    foreach (var field in descriptor.Fields.InDeclarationOrder())
                    {
                        if (reqData.Data.ContainsKey(field.Name))
                        {
                            field.Accessor.SetValue(reqTemp, reqData.Data[field.Name]);
                        }
                        else if (reqData.Data.ContainsKey(field.JsonName))
                        {
                            field.Accessor.SetValue(reqTemp, reqData.Data[field.JsonName]);
                        }

                    }
                }


                message.Data = reqTemp.ToByteArray();

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "从HTTP请求中解析数据错误:" + ex.Message);
                message = null;
            }

            return message;

        }
        /// <summary>
        /// 返回协议调用者
        /// </summary>
        /// <param name="rpcClient"></param>
        /// <returns></returns>
        protected override CallInvoker<AmpMessage> GetProtocolCallInvoker(IRpcClient<AmpMessage> rpcClient)
        {
            return new AmpCallInvoker(rpcClient);
        }

        protected override string MessageToJson(AmpMessage message)
        {
            if(message.Code == 0)
            {
                var return_code = 0;
                var return_message = "";
                string ret = "";
                if (message != null)
                {                  
                    var rspTemp = ProtobufObjectFactory.GetResponseTemplate(message.ServiceId, message.MessageId);
                    if (rspTemp == null)
                    {
                        return ret;
                    }

                    if (message.Data != null)
                    {
                        rspTemp.MergeFrom(message.Data);
                    }
                    //提取内部的return_code 字段
                    var field_code = rspTemp.Descriptor.FindFieldByName("return_code");
                    if(field_code != null)
                    {
                        var retObjV = field_code.Accessor.GetValue(rspTemp);
                        if(retObjV != null)
                        {
                            if(!int.TryParse(retObjV.ToString(), out return_code))
                            {
                                return_code = 0;
                            }
                        }
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

                return "{\"return_code\":0,\"return_message\":\"\",data:"+ret+"}";
            }
            else
            {
                return "{\"return_code\":"+message.Code+",\"return_message\":\"\"}";
            }

          
        }
    }
}
