
using DotBPE.Extra;
using DotBPE.Gateway;
using DotBPE.Rpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace GreeterHttpService
{
    public class Startup
    {
        public Startup(IConfiguration config)
        {
            this.Configuration = config;
        }

        IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //添加路由信息
            services.BindService<GreeterService>();
            services.AddGateway("GreeterHttpService");
            services.AddMessagePackSerializer();
            services.AddJsonNetParser();
            services.AddDynamicClientProxy();
            services.AddDynamicServiceProxy();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            //使用网关
            app.UseGateway();
        }
    }
}
