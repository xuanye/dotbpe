// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;
using Microsoft.Extensions.DependencyInjection;
using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using Microsoft.Extensions.Hosting;

namespace DotBPE.Extra
{
    /// <summary>
    /// This type is obsolete. Use DotBPE.Extensions.TextJsonSerializer instead.
    /// </summary>
    [Obsolete("Use DotBPE.Extensions.TextJsonSerializer instead")]
    public class TextJsonSerializer : Extensions.TextJsonSerializer
    {
    }

    /// <summary>
    /// This type is obsolete. Use DotBPE.Extensions.TextJsonParser instead.
    /// </summary>
    [Obsolete("Use DotBPE.Extensions.TextJsonParser instead")]
    public class TextJsonParser : Extensions.TextJsonParser
    {
    }

    /// <summary>
    /// This type is obsolete. Use DotBPE.Extensions.ServiceCollectionExtensions instead.
    /// </summary>
    [Obsolete("Use DotBPE.Extensions.ServiceCollectionExtensions instead")]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// This method is obsolete. Use DotBPE.Extensions.ServiceCollectionExtensions.AddTextJsonSerializer instead.
        /// </summary>
        [Obsolete("Use DotBPE.Extensions.ServiceCollectionExtensions.AddTextJsonSerializer instead")]
        public static IServiceCollection AddTextJsonSerializer(this IServiceCollection services)
        {
            return services.AddTextJsonSerializer();
        }

        /// <summary>
        /// This method is obsolete. Use DotBPE.Extensions.ServiceCollectionExtensions.AddTextJsonParser instead.
        /// </summary>
        [Obsolete("Use DotBPE.Extensions.ServiceCollectionExtensions.AddTextJsonParser instead")]
        public static IServiceCollection AddTextJsonParser(this IServiceCollection services)
        {
            return services.AddTextJsonParser();
        }
    }

    /// <summary>
    /// This type is obsolete. Use DotBPE.Extensions.ClientProxyFactoryExtensions instead.
    /// </summary>
    [Obsolete("Use DotBPE.Extensions.ClientProxyFactoryExtensions instead")]
    public static class ClientProxyFactoryExtensions
    {
        /// <summary>
        /// This method is obsolete. Use DotBPE.Extensions.ClientProxyFactoryExtensions.UseJsonNetSerializer instead.
        /// </summary>
        [Obsolete("Use DotBPE.Extensions.ClientProxyFactoryExtensions.UseJsonNetSerializer instead")]
        public static IClientProxyFactory UseJsonNetSerializer(this IClientProxyFactory @this)
        {
            return @this.UseJsonNetSerializer();
        }
    }

    /// <summary>
    /// This type is obsolete. Use DotBPE.Extensions.HostBuilderExtensions instead.
    /// </summary>
    [Obsolete("Use DotBPE.Extensions.HostBuilderExtensions instead")]
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// This method is obsolete. Use DotBPE.Extensions.HostBuilderExtensions.UseJsonNetSerializer instead.
        /// </summary>
        [Obsolete("Use DotBPE.Extensions.HostBuilderExtensions.UseJsonNetSerializer instead")]
        public static IHostBuilder UseJsonNetSerializer(this IHostBuilder @this)
        {
            return @this.UseJsonNetSerializer();
        }
    }
}
