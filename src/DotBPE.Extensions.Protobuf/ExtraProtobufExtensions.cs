// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;

namespace DotBPE.Extra
{
    /// <summary>
    /// Protobuf serializer for DotBPE RPC.
    /// </summary>
    [Obsolete("Use DotBPE.Extensions.Protobuf.ProtobufSerializer instead")]
    public class ProtobufSerializer : DotBPE.Extensions.Protobuf.ProtobufSerializer
    {
    }

    /// <summary>
    /// JSON parser for Google Protobuf messages.
    /// </summary>
    [Obsolete("Use DotBPE.Extensions.Protobuf.JsonParser instead")]
    public class JsonParser : DotBPE.Extensions.Protobuf.JsonParser
    {
    }
}

namespace DotBPE.Extra
{
    using Microsoft.Extensions.DependencyInjection;
    using DotBPE.Rpc;
    using DotBPE.Extensions.Protobuf;

    /// <summary>
    /// ServiceCollection extension methods for adding Protobuf serializer.
    /// </summary>
    [Obsolete("Use DotBPE.Extensions.Protobuf.ServiceCollectionExtensions instead")]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Protobuf serializer to the service collection.
        /// </summary>
        [Obsolete("Use DotBPE.Extensions.Protobuf.ServiceCollectionExtensions.AddProtobufSerializer instead")]
        public static IServiceCollection AddProtobufSerializer(this IServiceCollection services)
        {
            return DotBPE.Extensions.Protobuf.ServiceCollectionExtensions.AddProtobufSerializer(services);
        }

        /// <summary>
        /// Adds Protobuf serializer and JSON parser to the service collection.
        /// </summary>
        [Obsolete("Use DotBPE.Extensions.Protobuf.ServiceCollectionExtensions.AddProtobufSerializerAndJsonParser instead")]
        public static IServiceCollection AddProtobufSerializerAndJsonParser(this IServiceCollection services)
        {
            return DotBPE.Extensions.Protobuf.ServiceCollectionExtensions.AddProtobufSerializerAndJsonParser(services);
        }
    }
}

namespace DotBPE.Extra
{
    using DotBPE.Rpc.Client;
    using DotBPE.Extensions.Protobuf;

    /// <summary>
    /// ClientProxyFactory extension methods for Protobuf serializer.
    /// </summary>
    [Obsolete("Use DotBPE.Extensions.Protobuf.ClientProxyFactoryExtensions instead")]
    public static class ClientProxyFactoryExtensions
    {
        /// <summary>
        /// Configures the client proxy factory to use Protobuf serializer.
        /// </summary>
        [Obsolete("Use DotBPE.Extensions.Protobuf.ClientProxyFactoryExtensions.UseProtobufSerializer instead")]
        public static IClientProxyFactory UseProtobufSerializer(this IClientProxyFactory @this)
        {
            return DotBPE.Extensions.Protobuf.ClientProxyFactoryExtensions.UseProtobufSerializer(@this);
        }
    }
}

namespace DotBPE.Extra
{
    using Microsoft.Extensions.Hosting;
    using DotBPE.Extensions.Protobuf;

    /// <summary>
    /// HostBuilder extension methods for Protobuf serializer.
    /// </summary>
    [Obsolete("Use DotBPE.Extensions.Protobuf.HostBuilderExtensions instead")]
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Configures the host builder to use Protobuf serializer.
        /// </summary>
        [Obsolete("Use DotBPE.Extensions.Protobuf.HostBuilderExtensions.UseProtobufSerializer instead")]
        public static IHostBuilder UseProtobufSerializer(this IHostBuilder @this, bool useJsonParser = false)
        {
            return DotBPE.Extensions.Protobuf.HostBuilderExtensions.UseProtobufSerializer(@this, useJsonParser);
        }
    }
}
