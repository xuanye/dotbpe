// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;
using System.Collections.Generic;

namespace DotBPE.Rpc.Client.RoutingPolicies
{
    public class RandomRoutingPolicy : IRoutingPolicy
    {
        private static Random Random = new Random(100000);
        public IRouterPoint Select(string serviceKey, List<IRouterPoint> remoteAddresses)
        {
            int v = Random.Next();
            int index = v % remoteAddresses.Count;
            return remoteAddresses[index];
        }
    }
}
