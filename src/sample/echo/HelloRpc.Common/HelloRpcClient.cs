using System.Threading.Tasks;
using DotBPE.Rpc;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc.Exceptions;
using Google.Protobuf;


namespace HelloRpc.Common
{
    public sealed class GreeterClient : AmpInvokeClient
    {
        public GreeterClient(IMessageSender<AmpMessage> sender) : base(sender)
        {

        }

        public async Task<HelloResponse> HelloPlusAsnyc(HelloRequest request)
        {
            AmpMessage message = AmpMessage.CreateRequestMessage(100, 1);
            message.Data = request.ToByteArray();

            var response = await base.CallInvoker.AsyncCall(message);
            if (response != null && response.Data !=null)
            {
               return HelloResponse.Parser.ParseFrom(response.Data);
            }
            throw new RpcException("请求出错，请检查!");
        }

        public HelloResponse HelloPlus(HelloRequest request)
        {
            AmpMessage message = AmpMessage.CreateRequestMessage(100, 1);
            message.Data = request.ToByteArray();

            var response = base.CallInvoker.BlockingCall(message);
            if (response != null && response.Data != null)
            {
                return HelloResponse.Parser.ParseFrom(response.Data);
            }
            throw new RpcException("请求出错，请检查!");
        }
    }

    public sealed class MathClient : AmpInvokeClient
    {
        public MathClient(IMessageSender<AmpMessage> sender) : base(sender)
        {

        }

        public async Task<addResponse> AddAsnyc(addRequest request)
        {
            AmpMessage message = AmpMessage.CreateRequestMessage(101, 1);
            message.Data = request.ToByteArray();

            var response = await base.CallInvoker.AsyncCall(message);
            if (response != null && response.Data !=null)
            {
               return addResponse.Parser.ParseFrom(response.Data);
            }
            throw new RpcException("请求出错，请检查!");
        }

        public addResponse Add(addRequest request)
        {
            AmpMessage message = AmpMessage.CreateRequestMessage(101, 1);
            message.Data = request.ToByteArray();

            var response = base.CallInvoker.BlockingCall(message);
            if (response != null && response.Data != null)
            {
                return addResponse.Parser.ParseFrom(response.Data);
            }
            throw new RpcException("请求出错，请检查!");
        }
    }
}
