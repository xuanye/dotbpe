// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Gateway.Internal;
using DotBPE.Gateway.Swagger;
using DotBPE.Gateway.Tests.TestObjects;
using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DotBPE.Gateway.Tests
{
    public class ServiceExtensionsTests
    {
        [Fact]
        public void AddHttpApi_DefaultOption_PopulatedProperties_ShouldBeOk()
        {
            // arrange
            var services = new ServiceCollection();

            //act
            services.AddHttpApi();

            // assert
            var serviceProvider = services.BuildServiceProvider();
            var options1 = serviceProvider.GetRequiredService<RpcGatewayOption>();
            Assert.NotNull(options1);
            Assert.Equal(RpcGatewayOption.Default.CodeFieldName, options1.CodeFieldName);
            Assert.Equal(RpcGatewayOption.Default.DataFieldName, options1.DataFieldName);
            Assert.Equal(RpcGatewayOption.Default.MessageFieldName, options1.MessageFieldName);

            var options2 = serviceProvider.GetRequiredService<RpcGatewayOption>();

            Assert.Equal(options1, options2);
        }


        [Fact]
        public void AddHttpApi_OverrideOption_PopulatedProperties_ShouldBeOk()
        {
            // arrange
            var services = new ServiceCollection();

            var overrideOption = new RpcGatewayOption()
            {
                CodeFieldName = "return_code",
                DataFieldName = "data",
                MessageFieldName = "return_message",
            };

            //act
            services.AddHttpApi((o) =>
            {
                o.CodeFieldName = overrideOption.CodeFieldName;
                o.DataFieldName = overrideOption.DataFieldName;
                o.MessageFieldName = overrideOption.MessageFieldName;
            });

            // assert
            var serviceProvider = services.BuildServiceProvider();
            var options1 = serviceProvider.GetRequiredService<RpcGatewayOption>();
            Assert.NotNull(options1);
            Assert.Equal(overrideOption.CodeFieldName, options1.CodeFieldName);
            Assert.Equal(overrideOption.DataFieldName, options1.DataFieldName);
            Assert.Equal(overrideOption.MessageFieldName, options1.MessageFieldName);

            var options2 = serviceProvider.GetRequiredService<RpcGatewayOption>();

            Assert.Equal(options1, options2);
        }


        [Fact]
        public void AddHttpApi_CheckRequiredService_ShouldBeOk()
        {
            // arrange
            var services = new ServiceCollection();

            var clientProxy = new Moq.Mock<IClientProxy>();
            var jsonParser = new Moq.Mock<IJsonParser>();
            //act
            services.TryAddSingleton(clientProxy.Object);
            services.TryAddSingleton(jsonParser.Object);

            services.AddLogging();
            services.AddHttpApi();

            // assert
            var serviceProvider = services.BuildServiceProvider();
            var routeBuilder = serviceProvider.GetRequiredService<ApiRouteBuilder<ITestService>>();
            Assert.NotNull(routeBuilder);

        }



    }

}
