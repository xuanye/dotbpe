// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Hosting;
using DotBPE.Rpc.Protocols;
using DotBPE.Rpc.Server;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Peach;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DotBPE.Rpc.Tests.Hosting
{
    public class PeachServerHostTest
    {
        [Fact]
        public async Task RunServerHost_ShouldBeOK()
        {
            var services = new ServiceCollection();

            //协议
            //services.AddSingleton<IChannelHandlerPipeline, AmpChannelHandlerPipeline>();
            services.AddSingleton(new Mock<IChannelHandlerPipeline>().Object);

            //挂载服务逻辑
            services.AddSingleton<IServerHost, PeachServerHost>();
            //添加挂载的宿主服务
            //services.AddSingleton<IServerBootstrap, PeachServerBootstrap>();
            services.AddSingleton(new Mock<IServerBootstrap>().Object);

            //挂载服务逻辑
            services.AddSingleton(new Mock<ISocketService<AmpMessage>>().Object);

            //序列化模拟
            services.AddSingleton(new Mock<ISerializer>().Object);

            services.AddLogging();

            var host = services.BuildServiceProvider().GetRequiredService<IServerHost>();

            await host.StartAsync(CancellationToken.None);
            await host.StopAsync(CancellationToken.None);
        }

        [Fact]
        public async Task RunPeachServerHost_ShouldBeOK()
        {
            var services = new ServiceCollection();

            //协议
            services.AddSingleton<IChannelHandlerPipeline, AmpChannelHandlerPipeline>();


            //挂载服务逻辑
            services.AddSingleton<IServerHost, PeachServerHost>();

            //添加挂载的宿主服务
            services.AddSingleton<IServerBootstrap, PeachServerBootstrap>();


            //挂载服务逻辑
            services.AddSingleton(new Mock<IMessageHandler<AmpMessage>>().Object);
            services.AddSingleton<ISocketService<AmpMessage>, AmpPeachSocketService>();

            //序列化模拟
            services.AddSingleton(new Mock<ISerializer>().Object);

            services.AddLogging();

            var host = services.BuildServiceProvider().GetRequiredService<IServerHost>();

            await host.StartAsync(CancellationToken.None);
            await host.StopAsync(CancellationToken.None);
        }
    }
}
