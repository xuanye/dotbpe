using System;
using Microsoft.Extensions.DependencyInjection;
using DotBPE.Rpc;

namespace DotBPE.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJsonNetSerializer(this IServiceCollection services)
        {
            return services
                .AddSingleton<ISerializer, JsonSerializer>();
        }

        public static IServiceCollection AddJsonNetParser(this IServiceCollection services)
        {
            return services
                .AddSingleton<IJsonParser, JsonParser>();
        }
    }
}

namespace DotBPE.Extra
{
    /// <summary>
    /// ServiceCollectionExtensions is obsolete. Use DotBPE.Extensions.ServiceCollectionExtensions instead.
    /// </summary>
    [Obsolete("Use DotBPE.Extensions.ServiceCollectionExtensions instead")]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// AddJsonNetSerializer is obsolete. Use DotBPE.Extensions.ServiceCollectionExtensions.AddJsonNetSerializer instead.
        /// </summary>
        [Obsolete("Use DotBPE.Extensions.ServiceCollectionExtensions.AddJsonNetSerializer instead")]
        public static IServiceCollection AddJsonNetSerializer(this IServiceCollection services)
        {
            return DotBPE.Extensions.ServiceCollectionExtensions.AddJsonNetSerializer(services);
        }

        /// <summary>
        /// AddJsonNetParser is obsolete. Use DotBPE.Extensions.ServiceCollectionExtensions.AddJsonNetParser instead.
        /// </summary>
        [Obsolete("Use DotBPE.Extensions.ServiceCollectionExtensions.AddJsonNetParser instead")]
        public static IServiceCollection AddJsonNetParser(this IServiceCollection services)
        {
            return DotBPE.Extensions.ServiceCollectionExtensions.AddJsonNetParser(services);
        }
    }
}
