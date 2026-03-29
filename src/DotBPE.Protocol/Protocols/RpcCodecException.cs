// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;

namespace DotBPE.Protocol
{
    /// <summary>
    /// RPC codec exception
    /// </summary>
    public class RpcCodecException : Exception
    {
        /// <summary>
        /// Creates a new instance of RpcCodecException
        /// </summary>
        public RpcCodecException() : base()
        {
        }

        /// <summary>
        /// Creates a new instance of RpcCodecException with message
        /// </summary>
        public RpcCodecException(string message) : base(message)
        {
        }

        /// <summary>
        /// Creates a new instance of RpcCodecException with message and inner exception
        /// </summary>
        public RpcCodecException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
