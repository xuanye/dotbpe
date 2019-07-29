using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Tomato.Rpc.Client.RouterPolicy
{
    public class RoundrobinPolicy : IRouterPolicy
    {
        private static ConcurrentDictionary<string, int> ROUND_CACHE = new ConcurrentDictionary<string, int>();

        public IRouterPoint Select(string serviceKey, List<IRouterPoint> remoteAddresses)
        {
            if (!ROUND_CACHE.TryGetValue(serviceKey, out int index))
            {
                ROUND_CACHE.TryAdd(serviceKey, index);
            }
            else
            {
                ROUND_CACHE.TryUpdate(serviceKey, index + 1, index);
                index++;
            }
            index %= remoteAddresses.Count;
            
            return remoteAddresses[index];
        }
    }
}
