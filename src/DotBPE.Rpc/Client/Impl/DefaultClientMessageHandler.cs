// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Protocols;
using System.Collections.Generic;
using System.Linq;

namespace DotBPE.Rpc.Client
{
    public class DefaultClientMessageHandler : IClientMessageHandler
    {

        private readonly IMessageSubscriberContainer _subscriberContainer;

        public DefaultClientMessageHandler(IMessageSubscriberContainer subscriberContainer)
        {
            _subscriberContainer = subscriberContainer;
        }

        public void RaiseReceive(AmpMessage message)
        {
            var subscribers = _subscriberContainer.GetMessageSubscribers();
            if (subscribers?.Count > 0)
            {
                foreach (var subscriber in subscribers)
                {
                    subscriber.Handle(message);
                }
            }
        }
    }
}
