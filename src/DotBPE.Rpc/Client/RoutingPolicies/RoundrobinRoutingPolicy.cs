// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DotBPE.Rpc.Client.RoutingPolicies
{
    public class RoundrobinRoutingPolicy : IRoutingPolicy
    {
        private static ConcurrentDictionary<string, int> RoundCache = new ConcurrentDictionary<string, int>();

        public IRouterPoint Select(string serviceKey, List<IRouterPoint> remoteAddresses)
        {
            if (!RoundCache.TryGetValue(serviceKey, out int index))
            {
                RoundCache.TryAdd(serviceKey, index);
            }
            else
            {
                RoundCache.TryUpdate(serviceKey, index + 1, index);
                index++;
            }
            index %= remoteAddresses.Count;

            return remoteAddresses[index];
        }
    }
}
