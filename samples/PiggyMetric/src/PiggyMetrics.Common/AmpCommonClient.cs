using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using DotBPE.Rpc.Exceptions;
using Google.Protobuf;

namespace PiggyMetrics.Common
{
    public class AmpCommonClient : AmpInvokeClient
    {
        static readonly JsonFormatter AmpJsonFormatter = new JsonFormatter( new JsonFormatter.Settings(true));

        public AmpCommonClient(IRpcClient<AmpMessage> client) : base(client)
        {

        }

        public async Task<CallResult> SendAsync(ushort serviceId,ushort messageId ,string bodyContent,Dictionary<string,string> routerData,int timeOut = 3000)
        {
            CallResult result = new CallResult();
            AmpMessage request = AmpMessage.CreateRequestMessage(serviceId,messageId);
            IMessage reqTemp= ProtobufObjectFactory.GetRequestTemplate(serviceId,messageId);
            if(reqTemp ==null){
                result.Status = -1;
                result.Message = "request message undefined";
                return result;
            }


            try
            {
                var descriptor = reqTemp.Descriptor;
                if (!string.IsNullOrEmpty(bodyContent))
                {
                    reqTemp = descriptor.Parser.ParseJson(bodyContent);
                }

                if (routerData.Count > 0)
                {
                    foreach (var field in descriptor.Fields.InDeclarationOrder())
                    {
                        if (routerData.ContainsKey(field.Name))
                        {
                            field.Accessor.SetValue(reqTemp, routerData[field.Name]);
                        }
                        else if (routerData.ContainsKey(field.JsonName))
                        {
                            field.Accessor.SetValue(reqTemp, routerData[field.JsonName]);
                        }

                    }
                }


                request.Data = reqTemp.ToByteArray();

            }
            catch(Exception ex){
                result.Status = -1;
                result.Message = "code error:"+ex.Message+"\n"+ex.StackTrace;
                return result;
            }

            try{
                var rsp = await base.CallInvoker.AsyncCall(request,timeOut);
                if(rsp !=null){
                   var rspTemp = ProtobufObjectFactory.GetResponseTemplate(serviceId,messageId);
                    if(rspTemp ==null){
                        result.Status = -1;
                        result.Message = "response message undefined";
                        return result;
                    }

                    if(rsp.Data !=null)
                    {
                        rspTemp.MergeFrom(rsp.Data);
                    }
                    result.Content = AmpJsonFormatter.Format(rspTemp);
                }
            }
            catch(RpcCommunicationException rpcEx){
                result.Status = -2;
                result.Message = "timeout"+rpcEx.Message+"\n"+rpcEx.StackTrace;
            }
            catch(Exception ex){
                result.Status = -1;
                result.Message = "call error:"+ex.Message;
            }
            return result;

        }
    }


}
