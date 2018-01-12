using DotBPE.Rpc.Codes;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DotBPE.Rpc
{
    public interface IClientBootstrap<TMessage> : IDisposable where TMessage : IMessage
    {
        Task<IRpcContext<TMessage>> ConnectAsync(EndPoint endpoint);

        event EventHandler<DisConnectedArgs> DisConnected;
    }

    public class DisConnectedArgs
    {
        public EndPoint EndPoint { get; set; }

        public string ContextId { get; set; }
    }
}
