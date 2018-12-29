using DotBPE.Rpc.Codec;
using DotBPE.Rpc.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc
{
    public class RpcResult
    {
        public int Code { get; set; }
    }

    public class RpcResult<T> : RpcResult //where T:ITransportMessage<AmpMessage>
    {
        public T Data { get; set; }
    }

}
