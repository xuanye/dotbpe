// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using Castle.DynamicProxy;
using DotBPE.Rpc;
using DotBPE.Rpc.Exceptions;
using System;
using System.Threading.Tasks;

namespace DotBPE.Extra
{
    internal class ReflectionHelper
    {
        public static (Type requestType, Type responseType) GetInvocationCallTypes(IInvocation invocation)
        {
            var returnType = invocation.Method.ReturnType;
            Type responseType;

            if (typeof(Task).IsAssignableFrom(returnType) && returnType.IsGenericType) //Task<RpcResult>
            {
                var innerType = returnType.GetGenericArguments()[0];
                if (typeof(RpcResult).IsAssignableFrom(innerType) && innerType.IsGenericType)
                {
                    responseType = innerType.GetGenericArguments()[0];
                }
                else
                {
                    throw new RpcException("Return type must be Task<RpcResult<T>>");
                }
            }
            else
            {
                throw new RpcException("Return type must be Task<RpcResult<T>>");
            }


            return (invocation.Arguments[0].GetType(), responseType);
        }

    }
}
