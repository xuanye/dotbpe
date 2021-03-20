using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Foundatio.Messaging;
using System.Threading;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Options;

namespace DotBPE.BestPractice
{
    public abstract class AbsBizEventProcessorHostedService : IHostedService
    {

        private readonly IServiceProvider _provider;

        private readonly ILogger<AbsBizEventProcessorHostedService> _logger;

        private readonly Dictionary<string, List<IBizEventProcessor>> CACHE = new Dictionary<string, List<IBizEventProcessor>>();

        private readonly BizEventOptions _eventOptions;
        public AbsBizEventProcessorHostedService(IServiceProvider provider,IOptions<BizEventOptions> optionsAccess)
        {
            _provider = provider;
            _logger = provider.GetService<ILogger<AbsBizEventProcessorHostedService>>();

            _eventOptions = optionsAccess.Value ?? new BizEventOptions() { EventProcessDllPattern = "*.dll" };
        }

        protected abstract IMessageBus MessageBus { get; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            //step1 scan 
            ScanEventProcessor();
            if (CACHE.Count == 0)
            {
                _logger.LogInformation("没有发现BizEventProcessor，服务停止");
                return Task.CompletedTask;
            }
            MessageBus.SubscribeAsync<BizEventWrapper>(Consume, cancellationToken);
            _logger.LogInformation("BizEventProcessorHostedService is Started!");
            return Task.CompletedTask;
        }

        private void ScanEventProcessor()
        {
            string BaseDirectory = DotBPE.Rpc.Internal.Environment.GetAppBasePath();

            var dllFiles = Directory.GetFiles(string.Concat(BaseDirectory, ""), _eventOptions.EventProcessDllPattern);
            List<Assembly> assemblies = new List<Assembly>();
            foreach (var file in dllFiles)
            {
                assemblies.Add(Assembly.LoadFrom(file));
            }

            var processorType = typeof(IBizEventProcessor);

            foreach (var a in assemblies)
            {
                //Console.WriteLine(a.FullName);
                foreach (var t in a.GetTypes())
                {
                    if (processorType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract) //t 实现了某接口
                    {
                        var p = (IBizEventProcessor)ActivatorUtilities.GetServiceOrCreateInstance(_provider,t);
                        if (CACHE.ContainsKey(p.ProcessorEventName))
                        {
                            CACHE[p.ProcessorEventName].Add(p);
                        }
                        else
                        {
                            CACHE.Add(p.ProcessorEventName, new List<IBizEventProcessor> { p });
                        }
                    }
                }
            }


        }
        private async Task Consume(BizEventWrapper data, CancellationToken cancellationToken)
        {
            if (CACHE.ContainsKey(data.EventName))
            {
                var l = CACHE[data.EventName];

                foreach (var p in l)
                {
                    await Handler(p, data.EventData).ConfigureAwait(false);
                }
            }
            else
            {
                _logger.LogDebug("任务不存在消费者");
            }

        }

        private async Task Handler(IBizEventProcessor processor, IBizEvent data)
        {
            try
            {
                var res = await processor.ProcessAsync(data);
                if (res.Code != 0)
                {
                    _logger.LogWarning("事件{2}消费失败:code={0},message={1}", res.Code, res.Message, data.Name);
                }
                else
                {
                    _logger.LogDebug("事件{0}消费成功", data.Name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "消费任务出错");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            CACHE.Clear();
            return Task.CompletedTask;
        }
    }
}
