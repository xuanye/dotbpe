using DotBPE.Extra;
using DotBPE.Gateway;
using DotBPE.Rpc.Config;
using MathService.Definition;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
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
          

            services.AddMessagePackSerializer(); //message pack serializer
            services.AddJsonNetParser(); // http result json parser
            services.AddDynamicClientProxy(); // aop client
            services.AddDynamicServiceProxy(); // aop service

            services.AddDotBPEHttpApi();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints => {

                endpoints.MapService<IMathService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Welcome to DotBPE RPC Service.");
                });
            });
        }
    }
}
