using System;
using System.Collections.Generic;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotBPE.Rpc
{
    public static class ClientProxyFactoryExtensions
    {
        public static IClientProxyFactory UseDefaultChannel(this IClientProxyFactory @this, string remoteAddress)
        {

            return @this.UseChannel("default", remoteAddress);

        }
        public static IClientProxyFactory UseChannel(this IClientProxyFactory @this,string groupName, string remoteAddress)
        {

            return @this.Configure<RouterPointOptions>(o =>
            {
                if (o.Categories == null)
                {
                    o.Categories = new List<GroupIdentifierOption>();
                }

                var channel = o.Categories.Find(x => x.GroupName == groupName);
                if (channel == null)
                {
                    channel = new GroupIdentifierOption {GroupName = groupName};
                    o.Categories.Add(channel);
                }
                channel.RemoteAddress = remoteAddress;

            });

        }
        public static IClientProxyFactory ConfigureLogging(this IClientProxyFactory @this,Action<ILoggingBuilder> configure)
        {
            return @this.AddDependencyServices(services => services.AddLogging(configure));
        }
    }
}
