// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Gateway;
using DotBPE.Gateway.Internal;
using DotBPE.Gateway.Swagger;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {


        public static IServiceCollection AddHttpApi(this IServiceCollection services, Action<RpcGatewayOption> configureOptions = null, Action<SwaggerOptions> configureSwaggerOptions = null)
        {

            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }


            if (configureOptions != null)
            {
                services.Configure(configureOptions);
                services.AddSingleton(p =>
                {
                    return p.GetRequiredService<IOptions<RpcGatewayOption>>().Value;
                });
            }
            else
            {
                services.TryAddSingleton(new RpcGatewayOption());
            }


            services.TryAddSingleton(typeof(ApiRouteBuilder<>));
            services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IApiMethodProvider<>), typeof(DefaultApiMethodProvider<>)));


            services.TryAddEnumerable(ServiceDescriptor.Transient<IApiDescriptionProvider, HttpApiDescriptionProvider>());

            // Register default description provider in case MVC is not registered
            services.TryAddSingleton<IApiDescriptionGroupCollectionProvider>(serviceProvider =>
            {
                var actionDescriptorCollectionProvider = serviceProvider.GetService<IActionDescriptorCollectionProvider>();
                var apiDescriptionProvider = serviceProvider.GetServices<IApiDescriptionProvider>();

                return new ApiDescriptionGroupCollectionProvider(
                    actionDescriptorCollectionProvider ?? new EmptyActionDescriptorCollectionProvider(),
                    apiDescriptionProvider);
            });

            if (configureSwaggerOptions != null)
                services.Configure(configureSwaggerOptions);

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
