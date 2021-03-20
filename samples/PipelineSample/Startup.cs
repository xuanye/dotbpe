using DotBPE.Extra;
using DotBPE.Gateway;
using DotBPE.Rpc;
using Microsoft.AspNetCore.Builder;
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

            services.AddGateway("PipelineSample"); //add gateway and auto scan router infos

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

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseGateway();
        }
    }
}
