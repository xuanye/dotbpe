using System;
using MessagePack;

namespace MathCommon
{
    [MessagePackObject]
    public class AddReq
    {
        [Key(0)]
        public int A {get;set;}

        [Key(1)]
        public int B {get;set;}
    }

    [MessagePackObject]
    public class AddRes{

        [Key(0)]
        public int C {get;set;}
    }
}
