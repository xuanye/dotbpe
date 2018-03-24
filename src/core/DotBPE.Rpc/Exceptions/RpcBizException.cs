#region copyright

// -----------------------------------------------------------------------
//  <copyright file="RpcException.cs” project="DotBPE.Rpc">
//    文件说明:
//     copyright@2017 xuanye 2017-04-08 8:45
//  </copyright>
// -----------------------------------------------------------------------

#endregion copyright

using System;

namespace DotBPE.Rpc.Exceptions
{
    public class RpcBizException : Exception
    {
        public RpcBizException() { }

        public RpcBizException(string message) : base(message) { }

        public RpcBizException(string message, Exception inner) : base(message, inner) { }
    }
}
