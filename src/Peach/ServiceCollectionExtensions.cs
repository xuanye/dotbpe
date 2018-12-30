using Peach.Config;
using Peach.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Peach
{
    public static class ServiceCollectionExtensions
    {       
        public static IServiceCollection AddTcpServer<TMessage>(this IServiceCollection services) 
            where TMessage:Messaging.IMessage
        {
            services.TryAddScoped<IHostedService, PeachHostedService>();

            return services.AddSingleton<IServerBootstrap, Tcp.TcpServerBootstrap<TMessage>>();         
        }
    }
}
