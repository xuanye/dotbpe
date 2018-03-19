using System;
using System.Collections.Generic;
using System.Text;
using DotBPE.Rpc.Codes;

namespace DotBPE.Rpc {
    public interface ILoadPolicy : IDisposable {
        ITransport<TMessage> FindTransport<TMessage> (string serviceIdentifier) where TMessage : InvokeMessage;
    }
}