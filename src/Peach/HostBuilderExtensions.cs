using Microsoft.Extensions.Hosting;
using Peach.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using Peach.Config;

namespace Peach
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseTcpServer<TMessage>(this IHostBuilder builder)
            where TMessage:IMessage
        {
            builder.ConfigureServices(services =>
            {
                services.AddTcpServer<TMessage>();
            });
            return builder;
        }
    }
}
