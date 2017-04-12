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

        public Task Receive(IRpcContext<AmpMessage> context, AmpMessage req)
        {
            if(req.MessageId == 1)
            {
                return this.ReceiveHelloPlusAsnyc(context, req);
            }
            return Task.CompletedTask;
        }

        private async Task ReceiveHelloPlusAsnyc(IRpcContext<AmpMessage> context, AmpMessage req)
        {
            var request = HelloRequest.Parser.ParseFrom(req.Data);
            var data = await HelloPlusAsnyc(request);
            var response = AmpMessage.CreateResponseMessage(req.ServiceId, req.MessageId);
            response.Sequence = req.Sequence;
            response.Data = data.ToByteArray();
            await context.SendAsync(response);
        }
        /// <summary>
        /// 实际需要去实现的方法
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public abstract Task<HelloResponse> HelloPlusAsnyc(HelloRequest request);
    }
    public abstract class MathBase: IServiceActor<AmpMessage>
    {
        public string Id
        {
           get {
                return "101$0";
           }
        }

        public Task Receive(IRpcContext<AmpMessage> context, AmpMessage req)
        {
            if(req.MessageId == 1)
            {
                return this.ReceiveAddAsnyc(context, req);
            }
            return Task.CompletedTask;
        }

        private async Task ReceiveAddAsnyc(IRpcContext<AmpMessage> context, AmpMessage req)
        {
            var request = addRequest.Parser.ParseFrom(req.Data);
            var data = await AddAsnyc(request);
            var response = AmpMessage.CreateResponseMessage(req.ServiceId, req.MessageId);
            response.Sequence = req.Sequence;
            response.Data = data.ToByteArray();
            await context.SendAsync(response);
        }
        /// <summary>
        /// 实际需要去实现的方法
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public abstract Task<addResponse> AddAsnyc(addRequest req);
    }
}
