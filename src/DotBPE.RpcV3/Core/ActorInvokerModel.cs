// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Abstractions;

namespace DotBPE.Rpc.Core
{
    public class ActorInvokerModel
    {

        public ActorInvokerModel(IMethod method, RequestDelegate requestDelegate)
        {

            RequestDelegate = requestDelegate;
            Method = method;
        }

        public IMethod Method {
            get;
        }


        public RequestDelegate RequestDelegate {
            get;
        }


    }
}
