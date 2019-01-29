using DotBPE.Extra;
using DotBPE.Gateway;
using DotBPE.Rpc.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace MathHttpClient
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

            services.Configure<RouterPointOptions>(router =>
            {
                router.Categories.Add(new GroupIdentifierOption
                {
                    GroupName = "default",
                    RemoteAddress = Peach.Infrastructure.IPUtility.GetLocalIntranetIP()+":5566"
                });
            });
            services.AddGateway("MathService.Definition"); //add gateway and auto scan router infos

            services.AddMessagePackSerializer(); //message pack serializer
            services.AddJsonNetParser(); // http result json parser
            services.AddDynamicClientProxy(); // aop client
            services.AddDynamicServiceProxy(); // aop service

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseGateway();
        }
    }
}
