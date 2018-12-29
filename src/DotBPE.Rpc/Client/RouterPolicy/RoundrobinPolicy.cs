using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Client.RouterPolicy
{
    public class RoundrobinPolicy : IRouterPolicy
    {
        private static ConcurrentDictionary<string, int> ROUND_CACHE = new ConcurrentDictionary<string, int>();

        public IRouterPoint Select(string serviceKey, List<IRouterPoint> remoteAddresses)
        {
            var index = 0;
            if(!ROUND_CACHE.TryGetValue(serviceKey,out index))
            {
                ROUND_CACHE.TryAdd(serviceKey, index);
            }
            else
            {
                ROUND_CACHE.TryUpdate(serviceKey, index+1,index);
                index = index + 1;
            }
            if(index > remoteAddresses.Count - 1)
            {
                index = 0;
            }
            return remoteAddresses[index];
        }
    }
}
