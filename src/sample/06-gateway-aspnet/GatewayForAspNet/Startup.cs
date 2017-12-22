using System;
using System.Threading.Tasks;
using DotBPE.Plugin.AspNetGateway;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc.Options;

namespace GatewayForAspNet
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {

        }
      
        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            //内部服务地址，也可以使用服务发现的方式，这里使用本地配置
            services.Configure<RemoteServicesOption>(opt =>
            {
                opt.Add(new ServiceOption()
                {
                    ServiceId = 10006,
                    MessageIds = "1",
                    RemoteAddress = "127.0.0.1:6201" // 可以配置多个地址，用逗号分隔
                });
            });

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


            //添加DotBPE的Amp协议支持
            services.AddAmp();

            //添加转发服务配置
            services.AddTransforClient<AmpMessage>();
          
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {  
            //使用网关
            app.UseGateWay();
        }


    }
}
