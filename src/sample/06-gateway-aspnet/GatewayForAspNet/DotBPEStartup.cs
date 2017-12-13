using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GatewayForAspNet
{
    public class DotBPEStartup : IStartup
    {
        /// <summary>
        /// 配置应用信息，默认可以不配置
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IAppBuilder app, IHostingEnvironment env)
        {
            // 一般用于配置应用启动时 需要加载的配置信息，
            // 这里可以用过app 获取ConfigureServices 注入的服务
        }

        /// <summary>
        /// 配置需要加载的服务信息
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddDotBPE(); // 使用AMP协议

            //注册实际的服务
            services.AddServiceActors<AmpMessage>((actors) =>
            {
                actors.Add<GreeterService>();
            });

            return services.BuildServiceProvider();
        }
    }
}
