// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Logger = DotBPE.Gateway.Internal.ApiRouteBuilderLogger;


namespace DotBPE.Gateway.Internal
{

    public interface IApiRouteBuilder
    {
        IEnumerable<IEndpointConventionBuilder> Build(IEndpointRouteBuilder endpointRouteBuilder);
    }

    internal class ApiRouteBuilder<TService> : IApiRouteBuilder
        where TService : class
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<IApiMethodProvider<TService>> _apiMethodProviders;

        public ApiRouteBuilder(IEnumerable<IApiMethodProvider<TService>> apiMethodProviders, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ApiRouteBuilder<TService>>();
            _apiMethodProviders = apiMethodProviders;
        }
        public IEnumerable<IEndpointConventionBuilder> Build(IEndpointRouteBuilder endpointRouteBuilder)
        {
            Logger.DiscoveringServiceMethods(_logger, typeof(TService));

            var serviceMethodProviderContext = new ApiMethodProviderContext<TService>();
            foreach (var provider in _apiMethodProviders)
            {
                provider.OnMethodDiscovery(serviceMethodProviderContext);
            }

            var endpointConventionBuilders = new List<IEndpointConventionBuilder>();
            if (serviceMethodProviderContext.Methods.Count > 0)
            {
                foreach (var method in serviceMethodProviderContext.Methods)
                {
                    var endpointBuilder = endpointRouteBuilder.Map(method.Pattern, method.RequestDelegate);

                    endpointBuilder.Add(ep =>
                    {
                        ep.DisplayName = $"RPC - {method.Pattern.RawText}";

                        //ep.Metadata.Add(new ApiMethodMetadata(typeof(TService), method.Method));
                        foreach (var item in method.Metadata)
                        {
                            ep.Metadata.Add(item);
                        }
                    });

                    endpointConventionBuilders.Add(endpointBuilder);

                    Logger.AddedServiceMethod(_logger, method.Method.Name, method.Method.ServiceName, method.Pattern.RawText ?? string.Empty);
                }
            }
            else
            {
                Logger.NoServiceMethodsDiscovered(_logger, typeof(TService));
            }

            return endpointConventionBuilders;
        }
    }

    internal static class ApiRouteBuilderLogger
    {
        private static readonly Action<ILogger, string, string, string, Exception> _addedServiceMethod =
            LoggerMessage.Define<string, string, string>(LogLevel.Trace, new EventId(1, "AddedServiceMethod"), "Added RPC method '{MethodName}' to service '{ServiceName}'. route pattern: '{RoutePattern}'.");

        private static readonly Action<ILogger, Type, Exception> _discoveringServiceMethods =
            LoggerMessage.Define<Type>(LogLevel.Trace, new EventId(2, "DiscoveringServiceMethods"), "Discovering RPC methods for {ServiceType}.");

        private static readonly Action<ILogger, Type, Exception> _noServiceMethodsDiscovered =
            LoggerMessage.Define<Type>(LogLevel.Debug, new EventId(3, "NoServiceMethodsDiscovered"), "No RPC methods discovered for {ServiceType}.");

        public static void AddedServiceMethod(ILogger logger, string methodName, string serviceName, string routePattern)
        {
            _addedServiceMethod(logger, methodName, serviceName, routePattern, null);
        }

        public static void DiscoveringServiceMethods(ILogger logger, Type serviceType)
        {
            _discoveringServiceMethods(logger, serviceType, null);
        }

        public static void NoServiceMethodsDiscovered(ILogger logger, Type serviceType)
        {
            _noServiceMethodsDiscovered(logger, serviceType, null);
        }
    }
}
