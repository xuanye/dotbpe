#region copyright

// -----------------------------------------------------------------------
//  <copyright file="RpcException.cs” project="DotBPE.Rpc">
//    文件说明:
//     copyright@2017 xuanye 2017-04-08 8:44
//  </copyright>
// -----------------------------------------------------------------------

#endregion copyright

using System;

namespace DotBPE.Rpc.Exceptions
{
    public class RpcRemoteException : Exception
    {
        public RpcRemoteException()
        {
        }

        public RpcRemoteException(int code, string message) : base(message)
        {
            this.Code = code;
        }

        public RpcRemoteException(int code, string message, Exception inner) : base(message, inner)
        {
            this.Code = code;
        }

        public int Code { get; set; }
    }
}
