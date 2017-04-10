using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc
{
    public class DefaultCallInvoker<TMessage> : CallInvoker<TMessage> where TMessage:InvokeMessage
    {
        static readonly ILogger Logger = Environment.Logger.ForType<DefaultCallInvoker<TMessage>>();

        private readonly IRpcClient<TMessage> rpcClient;

        public DefaultCallInvoker(IRpcClient<TMessage> client)
        {
            this.rpcClient = client;
            this.rpcClient.Recieved += Message_Recieved;

        }

        private void Message_Recieved(object sender, MessageRecievedEventArgs<TMessage> e)
        {
            if(e.Message  ==null || e.Message.InvokeMessageType != InvokeMessageType.Response)
            {
                return;
            }
        }

        public override Task<TMessage> AsyncUnaryCall(TMessage request)
        {
            throw new NotImplementedException();
        }

        public override TMessage BlockingUnaryCall(TMessage request)
        {
            throw new NotImplementedException();
        }
    }
}
