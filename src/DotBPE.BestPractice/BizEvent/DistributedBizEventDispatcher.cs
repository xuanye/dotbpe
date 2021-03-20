using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Foundatio.Messaging;

namespace DotBPE.BestPractice
{

    public class DistributedBizEventDispatcher : IBizEventDispatcher
    {
        private readonly IMessageBus _messageBus;

        public DistributedBizEventDispatcher(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        public Task RaiseEvent(IBizEvent eventArgs)
        {
            return _messageBus.PublishAsync(new BizEventWrapper(eventArgs));
        }
    }

}
