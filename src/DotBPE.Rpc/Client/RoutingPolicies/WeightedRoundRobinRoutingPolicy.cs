// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DotBPE.Rpc.Client.RoutingPolicies
{
    internal class WeightedRoundRobinRoutingPolicy
    {
        private static ConcurrentDictionary<string, int> RoundCache = new ConcurrentDictionary<string, int>();

        public IRouterPoint Select(string serviceKey, List<IRouterPoint> remoteAddresses)
        {
            var max = TotalWeight(remoteAddresses);
            if (!RoundCache.TryGetValue(serviceKey, out int index))
            {
                RoundCache.TryAdd(serviceKey, index);
                return remoteAddresses[index];
            }
            else
            {
                RoundCache.TryUpdate(serviceKey, index + 1, index);
                index++;
            }

            if (index == max)
            {
                index = 0;
                return remoteAddresses[index];
            }

            int cur = 0;
            for (var i = 0; i < remoteAddresses.Count; i++)
            {
                cur += remoteAddresses[i].Weight;
                if (index <= cur)
                {
                    return remoteAddresses[i];
                }
            }
            return remoteAddresses[0];
        }



        private static int TotalWeight(List<IRouterPoint> remoteAddresses)
        {
            int total = 0;
            if (remoteAddresses.Count > 0)
            {
                for (var i = 0; i < remoteAddresses.Count; i++)
                {
                    total += remoteAddresses[i].Weight;
                }
            }
            return total;
        }
    }
}
