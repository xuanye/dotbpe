using DotBPE.Rpc.Codec;
using DotBPE.Rpc.Protocol;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MathService.Definition
{
    [DataContract]
    public class SumRes
    {
        [DataMember(Order =1,Name ="total")]
        public int Total { get; set; }
    }

    public class SumReq
    {
        [DataMember(Order = 1, Name = "a")]
        public int A { get; set; }
        [DataMember(Order = 2, Name = "b")]
        public int B { get; set; }
    }   
}
