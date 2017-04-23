using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Exceptions
{
    public class RpcCodecException: Exception
    {
        public RpcCodecException()
        {
            
        }

        public RpcCodecException(string message):base(message)
        {

        }
        public RpcCodecException(string message,Exception inner) : base(message, inner)
        {

        }
    }
}
