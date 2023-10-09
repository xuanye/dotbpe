// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Extra;
using DotBPE.Rpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace GreeterHttpService
{
    public class Startup
    {
        public Startup(IConfiguration config)
        {
            Configuration = config;
        }

        IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //添加路由信息
            //services.BindService<GreeterService>(); //bindService
            //services.BindService<SwaggerSampleService>();

            services.BindServices(this.GetType().Assembly, "default");

            services.AddMessagePackSerializer(); //message pack serializer
            services.AddTextJsonParser(); // http result json parser
            services.AddDynamicProxy(); // aop dynamic proxy      

            services.AddDotBPE();

            services.AddHttpApi(); //DotBPE HTTPAPI && Swagger support

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseStaticFiles();


            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseEndpoints(endpoints =>
            {
                endpoints.ScanMapServices();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Welcome to DotBPE RPC Service.");
                });
            });

        }
    }
}
