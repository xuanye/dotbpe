﻿// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

namespace DotBPE.Rpc.Server
{
    public class ActorInvokerModel
    {

        public ActorInvokerModel(IMethod method, RequestDelegate requestDelegate)
        {

            RequestDelegate = requestDelegate;
            Method = method;
        }

        public string MethodIdentifier => $"{Method.ServiceId}.{Method.MethodId}";
        public IMethod Method { get; }

        public RequestDelegate RequestDelegate { get; }
    }

}
