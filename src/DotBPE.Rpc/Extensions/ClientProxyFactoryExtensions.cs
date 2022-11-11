// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ClientProxyFactoryExtensions
    {
        public static IClientProxyFactory UseDefaultChannel(this IClientProxyFactory @this, string remoteAddress)
        {

            return @this.UseChannel("default", remoteAddress);

        }
        public static IClientProxyFactory UseChannel(this IClientProxyFactory @this, string groupName, string remoteAddress)
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
                    channel = new GroupIdentifierOption { GroupName = groupName };
                    o.Categories.Add(channel);
                }
                channel.RemoteAddress = remoteAddress;

            });

        }
        public static IClientProxyFactory ConfigureLogging(this IClientProxyFactory @this, Action<ILoggingBuilder> configure)
        {
            return @this.AddDependencyServices(services => services.AddLogging(configure));
        }
    }
}
