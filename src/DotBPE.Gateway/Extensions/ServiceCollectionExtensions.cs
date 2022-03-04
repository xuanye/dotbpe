using DotBPE.Gateway;
using DotBPE.Gateway.Internal;
using DotBPE.Gateway.Swagger;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Adds RPC HTTP API services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> for adding services.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddDotBPEHttpApi(this IServiceCollection services)
        {

            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAddSingleton(new RpcGatewayOption());
            services.TryAddSingleton(typeof(ServiceRouteBuilder<>));
            services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IRpcServiceMethodProvider<>), typeof(HttpApiServiceMethodProvider<>)));

            return services;
        }

        /// <summary>
        /// Adds RPC HTTP API services to the specified <see cref="IServiceCollection" />.
        /// </summary>   
        public static IServiceCollection AddDotBPESwagger(this IServiceCollection services, Action<SwaggerOptions> configureOptions = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddDotBPEHttpApi();

            services.TryAddEnumerable(ServiceDescriptor.Transient<IApiDescriptionProvider, RpcHttpApiDescriptionProvider>());

            // Register default description provider in case MVC is not registered
            services.TryAddSingleton<IApiDescriptionGroupCollectionProvider>(serviceProvider =>
            {
                var actionDescriptorCollectionProvider = serviceProvider.GetService<IActionDescriptorCollectionProvider>();
                var apiDescriptionProvider = serviceProvider.GetServices<IApiDescriptionProvider>();

                return new ApiDescriptionGroupCollectionProvider(
                    actionDescriptorCollectionProvider ?? new EmptyActionDescriptorCollectionProvider(),
                    apiDescriptionProvider);
            });

            if (configureOptions != null)
                services.Configure(configureOptions);

            services.TryAddSingleton<ISwaggerProvider, DefaultSwaggerProvider>();

            return services;
        }

        // Dummy type that is only used if MVC is not registered in the app
        private class EmptyActionDescriptorCollectionProvider : IActionDescriptorCollectionProvider
        {
            public ActionDescriptorCollection ActionDescriptors { get; } = new ActionDescriptorCollection(new List<ActionDescriptor>(), 1);
        }

    }
}
