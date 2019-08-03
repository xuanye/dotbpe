using Tomato.Rpc.Codec;
using Tomato.Rpc.Protocol;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Tomato.Rpc
{

    [DataContract]
    public class RpcResult
    {
        [DataMember(Order =1,Name ="return_code")]
        public int Code { get; set; }
    }
    [DataContract]
    public class RpcResult<T> : RpcResult //where T:ITransportMessage<AmpMessage>
    {
        [DataMember(Order = 3, Name = "data")]
        public T Data { get; set; }
    }

}
