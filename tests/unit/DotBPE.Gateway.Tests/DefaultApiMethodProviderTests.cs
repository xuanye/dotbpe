// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Gateway.Internal;
using DotBPE.Gateway.Tests.TestObjects;
using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace DotBPE.Gateway.Tests
{
    public class DefaultApiMethodProviderTests
    {

        [Fact]
        public void AddMethod_OptionGet_ResolveMethod()
        {
            // arrange & act
            var endpoints = MapEndpoints<ITestService>();


            var endpoint = FindApiEndpoint(endpoints, nameof(ITestService.GetAsync));
            var acceptVerb = endpoint.Metadata.GetMetadata<HttpApiMetadata>().HttpApiOptions.AcceptVerb;

            // assert
            Assert.Equal(HttpVerb.Get, acceptVerb);
            Assert.Equal("/api/test/{name}", endpoint.RoutePattern.RawText);
            Assert.Single(endpoint.RoutePattern.Parameters);
            Assert.Equal("name", endpoint.RoutePattern.Parameters[0].Name);

        }

        [Fact]
        public void AddMethod_OptionPost_ResolveMethod()
        {
            // arrange & act
            var endpoints = MapEndpoints<ITestService>();

            var endpoint = FindApiEndpoint(endpoints, nameof(ITestService.PostAsync));
            var acceptVerb = endpoint.Metadata.GetMetadata<HttpApiMetadata>().HttpApiOptions.AcceptVerb;

            // assert
            Assert.Equal(HttpVerb.Post, acceptVerb);
            Assert.Equal("/api/test", endpoint.RoutePattern.RawText);
            Assert.Empty(endpoint.RoutePattern.Parameters);
        }

        [Fact]
        public void AddMethod_NoHttpApiOptionInProto_ThrowNotFoundError()
        {
            // Arrange & Act           
            var endpoints = MapEndpoints<ITestService>();

            void action() => FindApiEndpoint(endpoints, nameof(ITestService.NotHttpAsync));

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(action);

            Assert.Equal("Couldn't find endpoint for method NotHttpAsync.", exception.Message);

        }

        [Fact]
        public void AddMethod_BadPattern_ThrowError()
        {
            // Arrange & Act
            void action() => MapEndpoints<IInvalidPatternTestService>();

            // Assert

            var invalidOperationException = Assert.Throws<InvalidOperationException>(action);

            Assert.Equal("Error binding RPC service To HttpApi 'IInvalidPatternTestService'.", invalidOperationException.Message);
            Assert.IsType<TargetInvocationException>(invalidOperationException.InnerException);
            Assert.IsType<InvalidOperationException>(invalidOperationException.InnerException.InnerException);

            Assert.Equal("Error binding BadPatternAsync on IInvalidPatternTestService to HTTP API.", invalidOperationException.InnerException.InnerException.Message);
            Assert.IsType<InvalidOperationException>(invalidOperationException.InnerException.InnerException.InnerException);
            Assert.Equal("Path template must start with /: api/test/{id}", invalidOperationException.InnerException.InnerException.InnerException.Message);
        }
        private static List<RouteEndpoint> FindApiEndpoints(IReadOnlyList<Endpoint> endpoints, string methodName)
        {
            var e = endpoints
                .Where(e => e.Metadata.GetMetadata<HttpApiMetadata>().HanderMethod.Name == methodName)
                .Cast<RouteEndpoint>()
                .ToList();

            return e;
        }
        private static RouteEndpoint FindApiEndpoint(IReadOnlyList<Endpoint> endpoints, string methodName)
        {
            var e = FindApiEndpoints(endpoints, methodName).FirstOrDefault();
            if (e == null)
            {
                throw new InvalidOperationException($"Couldn't find endpoint for method {methodName}.");
            }

            return e;
        }

        private IReadOnlyList<Endpoint> MapEndpoints<TService>()
         where TService : class
        {

            var clientProxy = new Moq.Mock<IClientProxy>();
            var jsonParser = new Moq.Mock<IJsonParser>();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            serviceCollection.AddSingleton(clientProxy.Object);
            serviceCollection.AddSingleton(jsonParser.Object);
            serviceCollection.AddSingleton(new RpcGatewayOption());
            serviceCollection.AddSingleton(typeof(ApiRouteBuilder<>));
            serviceCollection.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IApiMethodProvider<>), typeof(DefaultApiMethodProvider<>)));


            IEndpointRouteBuilder endpointRouteBuilder = new TestEndpointRouteBuilder(serviceCollection.BuildServiceProvider());
            endpointRouteBuilder.MapService<TService>();
            return endpointRouteBuilder.DataSources.Single().Endpoints;
        }

        private class TestEndpointRouteBuilder : IEndpointRouteBuilder
        {
            public ICollection<EndpointDataSource> DataSources { get; }
            public IServiceProvider ServiceProvider { get; }

            public TestEndpointRouteBuilder(IServiceProvider serviceProvider)
            {
                DataSources = new List<EndpointDataSource>();
                ServiceProvider = serviceProvider;
            }

            public IApplicationBuilder CreateApplicationBuilder()
            {
                return new ApplicationBuilder(ServiceProvider);
            }
        }
    }
}
