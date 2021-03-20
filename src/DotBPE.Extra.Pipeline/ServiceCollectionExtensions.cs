using Foundatio.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace DotBPE.Extra
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTaskQueueProducer(this IServiceCollection services)
        {
            services.TryAddSingleton<IRedisStorage,DefaultRedisStorage>();

            services.TryAddSingleton<IPipelineTaskService,DefaultPipelineTaskService>();
            services.TryAddSingleton<IDBStorage,InMemoryDbStorage>(); //内置持久化存储，测试用
            services.TryAddSingleton<IMessageBus,InMemoryMessageBus>(); //内置内存级别的消息通道，生产环境需要替换掉

            services.AddLogging();
            services.Configure<PipelineOptions>(o => { });//初始化配置信息

            return services;
        }

        public static IServiceCollection AddTaskQueueConsumer(this IServiceCollection services)
        {
            AddTaskQueueProducer(services);

            services.AddSingleton<IHostedService, DBStoragePullService>(); //DB拉取服务
            services.AddSingleton<IHostedService, RedisStoragePullService>(); //Redis拉取服务
            services.AddSingleton<IHostedService, MessageBusConsumeService>(); //消息消费服务
            return services;
        }
    }
}
