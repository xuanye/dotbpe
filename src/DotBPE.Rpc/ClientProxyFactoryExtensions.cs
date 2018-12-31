using System.Collections.Generic;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Config;

namespace DotBPE.Rpc
{
    public static class ClientProxyFactoryExtensions
    {
        public static IClientProxyFactory UseDefaultChannel(this IClientProxyFactory @this, string remoteAddress)
        {

            return @this.UseChannel("default", remoteAddress);

        }
        public static IClientProxyFactory UseChannel(this IClientProxyFactory @this,string categateName, string remoteAddress)
        {

            return @this.Configure<RouterPointOptions>(o =>
            {
                if (o.Categories == null)
                {
                    o.Categories = new List<CategoryIdentifierOption>();
                }

                var channel = o.Categories.Find(x => x.Category == categateName);
                if (channel == null)
                {
                    channel = new CategoryIdentifierOption {Category = categateName};
                    o.Categories.Add(channel);
                }
                channel.RemoteAddress = remoteAddress;

            });

        }
    }
}
