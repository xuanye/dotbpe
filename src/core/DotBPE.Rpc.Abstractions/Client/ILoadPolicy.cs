using System;

namespace DotBPE.Rpc
{
    public interface ILoadPolicy : IDisposable
    {
        ITransport<TMessage> FindTransport<TMessage>(string serviceIdentifier) where TMessage : InvokeMessage;
    }
}
