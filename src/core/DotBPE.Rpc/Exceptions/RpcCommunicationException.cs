#region copyright

// -----------------------------------------------------------------------
//  <copyright file="RpcException.cs” project="DotBPE.Rpc">
//    文件说明:
//     copyright@2017 xuanye 2017-04-08 8:46
//  </copyright>
// -----------------------------------------------------------------------

#endregion copyright

using System;

namespace DotBPE.Rpc.Exceptions {
    /// <summary>
    /// RPC通信异常类
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class RpcCommunicationException : Exception {
        public RpcCommunicationException () { }

        public RpcCommunicationException (string message) : base (message) { }

        public RpcCommunicationException (string message, Exception inner) : base (message, inner) { }
    }
}