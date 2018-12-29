using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotBPE.Rpc.Client.RouterPolicy
{
    public class WeightedRoundRobinPolicy : IRouterPolicy
    {
        private static ConcurrentDictionary<string, int> ROUND_CACHE = new ConcurrentDictionary<string, int>();

        public IRouterPoint Select(string serviceKey, List<IRouterPoint> remoteAddresses)
        {
            var index = 0;
            var max = TotalWeight(remoteAddresses);
            if (!ROUND_CACHE.TryGetValue(serviceKey, out index))
            {
                ROUND_CACHE.TryAdd(serviceKey, index);
                return remoteAddresses[index];
            }
            else
            {
                ROUND_CACHE.TryUpdate(serviceKey, index + 1, index);
                index = index + 1;
            }

            if(index == max)
            {
                index = 0;
                return remoteAddresses[index];
            }

            int cur = 0;
            for(var i =0; i < remoteAddresses.Count; i++)
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
            if (remoteAddresses.Any())
            {
                for(var i = 0; i < remoteAddresses.Count; i++)
                {
                    total += remoteAddresses[i].Weight;
                }
            }
            return total;
        }

    }
}
