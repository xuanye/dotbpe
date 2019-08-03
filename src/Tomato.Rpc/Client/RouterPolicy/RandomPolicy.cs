using System;
using System.Collections.Generic;
using System.Text;

namespace Tomato.Rpc.Client.RouterPolicy
{
    public class RandomPolicy : IRouterPolicy
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
