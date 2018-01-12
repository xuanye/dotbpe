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
using DotBPE.Rpc;

namespace Survey.AspNetGateway
{

    public class Startup
    {
        public Startup(Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
           .SetBasePath(env.ContentRootPath)
           .AddJsonFile("dotbpe.json", optional: true, reloadOnChange: true) //服务相关的配置
           .AddJsonFile($"dotbpe.{env.EnvironmentName}.json", optional: true)         
           .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            //内部服务地址，也可以使用服务发现的方式，这里使用本地配置
            services.Configure<RemoteServicesOption>(Configuration.GetSection("remoteServices"));

            //添加路由信息
            services.AddRoutes();

            // 自动转发服务
            services.AddScoped<IForwardService, ForwardService>();


            //添加DotBPE的Amp协议支持
            services.AddAmp();

            //添加转发服务配置
            services.AddTransforClient<AmpMessage>();
                        

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            //静态文件，默认目录是wwwroot
            app.UseStaticFiles();
           
            //使用网关
            app.UseGateWay();
           
        }


    }
}
