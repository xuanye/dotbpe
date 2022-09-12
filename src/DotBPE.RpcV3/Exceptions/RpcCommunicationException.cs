// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license


using System;

namespace DotBPE.Rpc.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    /// RPC通信异常类
    /// </summary>
    /// <seealso cref="T:System.Exception" />
    public class RpcCommunicationException : Exception
    {
        public RpcCommunicationException()
        {
        }

        public RpcCommunicationException(string message) : base(message)
        {
        }

        public RpcCommunicationException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
