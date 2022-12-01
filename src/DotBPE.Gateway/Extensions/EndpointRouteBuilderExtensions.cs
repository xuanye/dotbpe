// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license


using DotBPE.Gateway.Internal;
using DotBPE.Rpc;
using DotBPE.Rpc.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;



namespace Microsoft.AspNetCore.Routing
{
    public static class EndpointRouteBuilderExtensions
    {

        public static IEndpointConventionBuilder MapService<TService>(this IEndpointRouteBuilder builder) where TService : class
        {
            var routeBuilder = builder.ServiceProvider.GetRequiredService<ApiRouteBuilder<TService>>();

            var endpointConventionBuilders = routeBuilder.Build(builder);

            return new ApiEndpointConventionBuilder(endpointConventionBuilders);
        }

        private static IEndpointConventionBuilder MapService(this IEndpointRouteBuilder builder, Type serviceType)
        {
            var builderType = typeof(ApiRouteBuilder<>);
            var serviceRouteBuilder = builder.ServiceProvider.GetRequiredService(builderType.MakeGenericType(serviceType)) as IApiRouteBuilder;

            var endpointConventionBuilders = serviceRouteBuilder.Build(builder);

            return new ApiEndpointConventionBuilder(endpointConventionBuilders);
        }

        public static void ScanMapServices(this IEndpointRouteBuilder builder, params string[] categories)
        {
            var serviceActors = builder.ServiceProvider.GetServices<IServiceActor>();

            foreach (var actor in serviceActors)
            {
                var interfaces = actor.GetType().GetInterfaces();

                foreach (var type in interfaces)
                {

                    if (!type.IsInterface)
                        continue;

                    //this._logger.LogInformation(type.FullName);
                    var sAttr = type.GetCustomAttribute<RpcServiceAttribute>();
                    if (sAttr == null)
                        continue;

                    if ((categories.Any() && categories.Contains(sAttr.GroupName)) || "default".Equals(sAttr.GroupName, StringComparison.OrdinalIgnoreCase))
                    {
                        builder.MapService(type);
                    }

                }
            }

        }
    }
}
