using System;
using System.Collections.Generic;
using System.Text;
using Foundatio.Messaging;
using Microsoft.Extensions.Options;

namespace DotBPE.BestPractice
{
    public class MemoryBizEventProcessorHostedService:AbsBizEventProcessorHostedService
    {
        public MemoryBizEventProcessorHostedService(IServiceProvider provider, InMemoryMessageBus messageBus,IOptions<BizEventOptions> optionsAccess) : base(provider, optionsAccess)
        {
            MessageBus = messageBus;
        }

        protected override IMessageBus MessageBus { get; }
    }
}
