// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    public class DefaultServiceRouter : IServiceRouter
    {
        public Task<IRouterPoint> FindRouterPoint(string servicePath)
        {
            throw new NotImplementedException();
        }
    }
}
