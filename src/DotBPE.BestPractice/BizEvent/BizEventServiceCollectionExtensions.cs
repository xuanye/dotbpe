using Foundatio.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DotBPE.BestPractice
{
    public static class BizEventServiceCollectionExtensions
    {
        public static IServiceCollection AddMemoryBizEventDispatcher(this IServiceCollection services)
        {
            //注入默认的业务事件分发器
            services.TryAddSingleton<InMemoryMessageBus>();
            return services.AddSingleton<IBizEventDispatcher, MemoryBusBizEventDispatcher>();
        }

        public static IServiceCollection AddDistributedBizEventDispatcher(this IServiceCollection services)
        {
            //注入默认的业务事件分发器 内存
            services.TryAddSingleton<IMessageBus, InMemoryMessageBus>(); //默认假装分布式

            return services.AddSingleton<IBizEventDispatcher, DistributedBizEventDispatcher>();
        }

        public static IServiceCollection AddMemoryBizEventProcessor(this IServiceCollection services)
        {
            //注入默认的业务事件分发器
            services.TryAddSingleton<InMemoryMessageBus>();
            return services.AddHostedService<MemoryBizEventProcessorHostedService>();
        }

        public static IServiceCollection AddDistributedBizEventProcessor(this IServiceCollection services)
        {
            //注入默认的业务事件分发器 内存
            services.TryAddSingleton<IMessageBus, InMemoryMessageBus>(); //默认假装分布式

            return services.AddHostedService<DistributedBizEventProcessorHostedService>();
        }

        public static IServiceCollection AddMemoryBizEventDispatcherAndProcessor(this IServiceCollection services)
        {
            //注入默认的业务事件分发器
            services.TryAddSingleton<InMemoryMessageBus>();
            return services.AddSingleton<IBizEventDispatcher, MemoryBusBizEventDispatcher>()
                .AddHostedService<MemoryBizEventProcessorHostedService>();
        }

        public static IServiceCollection AddDistributedBizEventDispatcherAndProcessor(this IServiceCollection services)
        {
            //注入默认的业务事件分发器 内存
            services.TryAddSingleton<IMessageBus, InMemoryMessageBus>(); //默认假装分布式

            return services.AddSingleton<IBizEventDispatcher, DistributedBizEventDispatcher>()
            .AddHostedService<DistributedBizEventProcessorHostedService>();
        }
    }
}
