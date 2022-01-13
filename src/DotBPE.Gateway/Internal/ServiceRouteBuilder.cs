using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Log = DotBPE.Gateway.Internal.ServiceRouteBuilderLog;

namespace DotBPE.Gateway.Internal
{

    public interface IServiceRouteBuilder
    {
        IEnumerable<IEndpointConventionBuilder> Build(IEndpointRouteBuilder endpointRouteBuilder);
    }

    internal class ServiceRouteBuilder<TService>:IServiceRouteBuilder where TService : class
    {
        private readonly IEnumerable<IRpcServiceMethodProvider<TService>> _serviceMethodProviders;
       
        private readonly ILogger _logger;


        public ServiceRouteBuilder(
           IEnumerable<IRpcServiceMethodProvider<TService>> serviceMethodProviders,           
           ILoggerFactory loggerFactory)
        {
            _serviceMethodProviders = serviceMethodProviders.ToList();        
            _logger = loggerFactory.CreateLogger<ServiceRouteBuilder<TService>>();
        }

        public IEnumerable<IEndpointConventionBuilder> Build(IEndpointRouteBuilder endpointRouteBuilder)
        {
            Log.DiscoveringServiceMethods(_logger, typeof(TService));

            var serviceMethodProviderContext = new RpcServiceMethodProviderContext<TService>();
            foreach (var serviceMethodProvider in _serviceMethodProviders)
            {
                serviceMethodProvider.OnServiceMethodDiscovery(serviceMethodProviderContext);
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

                        ep.Metadata.Add(new RpcMethodMetadata(typeof(TService), method.Method));
                        foreach (var item in method.Metadata)
                        {
                            ep.Metadata.Add(item);
                        }
                    });

                    endpointConventionBuilders.Add(endpointBuilder);

                    Log.AddedServiceMethod(_logger, method.Method.Name, method.Method.ServiceName,method.Pattern.RawText ?? string.Empty);
                }
            }
            else
            {
                Log.NoServiceMethodsDiscovered(_logger, typeof(TService));
            }

            //NOTE:CreateUnimplementedEndpoints ??

            return endpointConventionBuilders;
        }

       
    }
    internal static class ServiceRouteBuilderLog
    {
        private static readonly Action<ILogger, string, string,  string, Exception> _addedServiceMethod =
            LoggerMessage.Define<string, string,  string>(LogLevel.Trace, new EventId(1, "AddedServiceMethod"), "Added RPC method '{MethodName}' to service '{ServiceName}'. route pattern: '{RoutePattern}'.");

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
