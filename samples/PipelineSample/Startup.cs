using DotBPE.Extra;
using DotBPE.Gateway;
using DotBPE.Rpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PipelineSample.Services;
using StackExchange.Redis;


namespace PipelineSample
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
            services.BindService<FooService>(); //bindService          
            services.BindService<QueueTaskService>();


            services.AddDotBPE();

            services.AddMessagePackSerializer(); //message pack serializer
            services.AddTextJsonParser(); // http result json parser
            services.AddDynamicClientProxy(); // aop client
            services.AddDynamicServiceProxy(); // aop service

            services.ScanAddRpcInvokerReflection("PipelineSample");

            services.AddTaskQueueConsumer();
            services.AddTaskQueueProducer();



            string redisString = Configuration.GetValue<string>("redis:configuration");

            var redis = ConnectionMultiplexer.Connect(redisString);
            services.AddSingleton(redis);

            services.AddDotBPESwagger();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {

            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseEndpoints(endpoints =>
            {
                endpoints.ScanMapServices("PipelineSample");

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Welcome to DotBPE RPC Service.");
                });
            });
        }
    }
}
