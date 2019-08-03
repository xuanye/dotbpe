using System;

namespace Tomato.Rpc.Exceptions
{
    /// <summary>
    /// codec exception
    /// </summary>
    public class RpcCodecException : Exception
    {
        public RpcCodecException()
        {
        }

        public RpcCodecException(string message) : base(message)
        {
        }

        public RpcCodecException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
