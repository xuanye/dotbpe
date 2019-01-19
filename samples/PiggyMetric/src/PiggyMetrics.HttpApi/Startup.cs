using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PiggyMetrics.Common;
using PiggyMetrics.Common.Consul.Service;
using PiggyMetrics.Common.Extension;

namespace PiggyMetrics.HttpApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            _localConfiguration = LocalConfig.Load();
            if (string.IsNullOrEmpty(_localConfiguration.ConsulServer))
            {
                throw new Exception("consul.sever 未配置");
            }

            var consulOptions = new ConsulConfigurationOptions
            {
                Key = _localConfiguration.AppName,
                ReloadOnChange = true,
                ConsulAddress = new Uri(_localConfiguration.ConsulServer)
            };

            var builder = new ConfigurationBuilder()
                .AddConsul(consulOptions);

            Configuration = builder.Build();

        }
        private readonly LocalConfig _localConfiguration;
        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<RouterOption>(Configuration.GetSection("RouterOption"));
            services.AddScoped<IForwardService,ForwardService>();
            // 添加服务发现
            services.AddSingleton<IServiceDiscovery>(new ConsulServiceDiscovery(_localConfiguration.AppName,_localConfiguration.RequireService, (config) =>
            {
                config.Address = new Uri(_localConfiguration.ConsulServer);
            }));

            //添加客户端的协议
            services.AddAmpConsulClient();

            services.AddAuthentication();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationScheme = _localConfiguration.AppName,
                AutomaticAuthenticate = true,
                AutomaticChallenge = true
            });
            if(!string.IsNullOrEmpty(_localConfiguration.RequireService)){
                app.UseConsuleDiscovery();
            }
            //认证中间件，判断是否登录和登录处理
            app.UseAuthenticate(new AuthenticateOption(){ LoginPath="/auth/login"});

            app.Run(Proccess);
        }

        public async Task Proccess(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            var result = await context.RequestServices.GetRequiredService<IForwardService>().ForwardAysnc(context);
            if(result.Status ==0){
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result.Content);
            }
            else if(result.Status == 404 )
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync(result.Message??"Service not found!");
            }
            else{
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync(result.Message??"Server Internal Error!");
            }
        }

    }
}
