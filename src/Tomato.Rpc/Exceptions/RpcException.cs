#region copyright

//-----------------------------------------------------------------------
// <copyright file="RpcException.cs” project="Tomato.Rpc">
//    文件说明:
//    copyright@2017 xuanye
// </copyright>
//-----------------------------------------------------------------------

#endregion copyright

using System;

namespace Tomato.Rpc.Exceptions
{
    public class RpcException : Exception
    {
        public RpcException()
        {
        }

        public RpcException(string message) : base(message)
        {
        }

        public RpcException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
