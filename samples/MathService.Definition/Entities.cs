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

        [DataMember(Order = 2, Name = "foo_word")]
        public string FooWord { get; set; }
    }

    [DataContract]
    public class SumReq
    {
        [DataMember(Order = 1, Name = "a")]
        public int A { get; set; }
        [DataMember(Order = 2, Name = "b")]
        public int B { get; set; }
    }
    [DataContract]
    public class FooReq
    {
        [DataMember(Order = 1, Name = "foo_word")]
        public string FooWord{ get; set; }
    }

    [DataContract]
    public class FooRes
    {
        [DataMember(Order = 1, Name = "ret_word")]
        public string RetWord { get; set; }
    }
}
