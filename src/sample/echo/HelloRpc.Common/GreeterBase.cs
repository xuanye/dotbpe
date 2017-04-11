using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;

namespace HelloRpc.Common
{

    /// <summary>
    /// 这里是RPC服务的服务端实现基类，代码自动生成
    /// </summary>
    public abstract class GreeterBase: IServiceActor<AmpMessage>
    {        
        public string Id
        {
           get {
                return "100$0";
           }            
        }

        public Task Receive(IRpcContext<AmpMessage> context, AmpMessage message)
        {
            if(message.MessageId == 1)
            {
                return this.ReceiveSayHelloAsnyc(context, message);
            }
            return Task.CompletedTask;
        }

        private async Task ReceiveSayHelloAsnyc(IRpcContext<AmpMessage> context, AmpMessage message)
        {
            var request = HelloRequest.Parser.ParseFrom(message.Data);
            var data = await SayHelloAsnyc(request);
            var response = AmpMessage.CreateResponseMessage(message.ServiceId, message.MessageId);
            response.Sequence = message.Sequence;
            response.Data = data.ToByteArray();
            await context.SendAsync(response);
        }
        /// <summary>
        /// 实际需要去实现的方法
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public abstract Task<HelloReply> SayHelloAsnyc(HelloRequest request); 
    }
}
