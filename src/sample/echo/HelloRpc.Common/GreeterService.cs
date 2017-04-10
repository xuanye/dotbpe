using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;

namespace HelloRpc.Common
{
    public class GreeterService : InvokeRemoteBaseService<AmpMessage>
    {
        public GreeterService(IRpcClient<AmpMessage> client) : base(client)
        {

        }
        public async Task<HelloReply> SayHelloAsnyc(HelloRequest request)
        {
            AmpMessage message = new AmpMessage();
            message.InvokeMessageType = DotBPE.Rpc.Codes.InvokeMessageType.Request;
            message.ServiceId = 100;
            message.MessageId = 1;
            message.Version = 0;
            message.Data = request.ToByteArray();

            var response = await base.CallInvoker.AsyncUnaryCall(message);
            if (response != null && response.Data !=null)
            {               
               return HelloReply.Parser.ParseFrom(response.Data); 
            }
            throw new RpcException("请求出错，请检查!");
        }

        public HelloReply SayHello(HelloRequest request)
        {
            AmpMessage message = new AmpMessage();
            message.InvokeMessageType = DotBPE.Rpc.Codes.InvokeMessageType.Request;
            message.ServiceId = 100;
            message.MessageId = 1;
            message.Version = 0;
            message.Data = request.ToByteArray();

            var response = base.CallInvoker.BlockingUnaryCall(message);
            if (response != null && response.Data != null)
            {
                return HelloReply.Parser.ParseFrom(response.Data);
            }
            throw new RpcException("请求出错，请检查!");
        }
    }
}
