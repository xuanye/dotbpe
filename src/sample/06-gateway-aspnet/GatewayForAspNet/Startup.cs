using DotBPE.Plugin.AspNetGateway;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GatewayForAspNet
{
    public class Startup
    {
        public Startup(IConfiguration config)
        {
            this.Configuration = config;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //路由配置,也可以使用配置文件哦
            services.Configure<HttpRouterOption>(
                (opt) =>
                {
                    opt.CookieMode = CookieMode.Manual; //手动

                    opt.Items = new System.Collections.Generic.List<HttpRouterOptionItem>();
                    //注册路由信息
                    opt.Items.Add(new HttpRouterOptionItem()
                    {
                        ServiceId = 10006,
                        MessageId = 1,
                        Path = "/api/greet"
                    });
                }
            );

            // 自动转发服务
            services.AddScoped<IForwardService, ForwardService>();

            //添加服务端支持
            services.AddDotBPE();

            services.AddServiceActors<AmpMessage>((actors) =>
            {
                actors.Add<GreeterService>();
            });

            //添加本地代理模式客户端
            services.AddAgentClient<AmpMessage>();

            //添加RPC服务
            services.AddSingleton<IHostedService, VirtualRpcHostService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            
            //使用网关
            app.UseGateWay();
        }
    }
}
