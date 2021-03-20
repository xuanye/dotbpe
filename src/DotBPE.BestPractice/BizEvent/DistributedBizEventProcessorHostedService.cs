using System;
using Foundatio.Messaging;
using Microsoft.Extensions.Options;

namespace DotBPE.BestPractice
{
    public class DistributedBizEventProcessorHostedService : AbsBizEventProcessorHostedService
    {
        public DistributedBizEventProcessorHostedService(IServiceProvider provider,IMessageBus messageBus, IOptions<BizEventOptions> optionsAccess) :base(provider, optionsAccess)
        {
            MessageBus = messageBus;
        }

        protected override IMessageBus MessageBus { get; }
    }
}
