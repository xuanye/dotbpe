// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Protocols;
using System;

namespace DotBPE.Rpc.Exceptions
{
    public class RpcException : Exception
    {
        public RpcException(string message) : this(RpcStatusCodes.CODE_INTERNAL_ERROR, message)
        {

        }
        public RpcException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public RpcException(int statusCode, string message, Exception inner) : base(message, inner)
        {
            StatusCode = statusCode;
        }
        public int StatusCode { get; set; }
    }
}
