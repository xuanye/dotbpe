// Generated by the protocol buffer compiler. DO NOT EDIT!
// source: math.proto
#region Designer generated code

using System;
using System.Threading.Tasks;
using DotBPE.Rpc;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc.Exceptions;
using Google.Protobuf;
using DotBPE.Rpc.Client;

namespace MathCommon {

    //start for class MathInnerClient
    public sealed class MathInnerClient : AmpInvokeClient
    {
        public MathInnerClient(ICallInvoker<AmpMessage> callInvoker) : base(callInvoker)
        {

        }

        //同步方法
        public RpcResult<AddRes> Plus(AddReq req)
        {
            AmpMessage message = AmpMessage.CreateRequestMessage(10006, 1);

            message.FriendlyServiceName = "MathInner.Plus";


            message.Data = req.ToByteArray();
            var response = base.CallInvoker.BlockingCall(message);
            if (response == null)
            {
                throw new RpcException("error,response is null !");
            }
            var result = new RpcResult<AddRes>();
            if (response.Code != 0)
            {
                result.Code = response.Code;
            }
            
            if (response.Data == null)
            {
                result.Data  =  new AddRes();
            }
            else
            {
                result.Data = AddRes.Parser.ParseFrom(response.Data);
            }

            return result;
        }

        public async Task<RpcResult<AddRes>> PlusAsync(AddReq req, int timeOut = 3000)
        {
            AmpMessage message = AmpMessage.CreateRequestMessage(10006, 1);
            message.FriendlyServiceName = "MathInner.Plus";
            message.Data = req.ToByteArray();
            var response = await base.CallInvoker.AsyncCall(message, timeOut);
            if (response == null)
            {
                throw new RpcException("error,response is null !");
            }
           var result = new RpcResult<AddRes>();
            if (response.Code != 0)
            {
                result.Code = response.Code;
            }
            
            if (response.Data == null)
            {
                result.Data = new AddRes();
            }
            else
            {
                result.Data = AddRes.Parser.ParseFrom(response.Data);
            }

            return result;
        }

    }
    //end for class MathInnerClient
}
#endregion