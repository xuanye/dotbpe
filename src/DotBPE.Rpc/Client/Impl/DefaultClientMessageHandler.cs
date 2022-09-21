// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Protocols;
using System.Collections.Generic;
using System.Linq;

namespace DotBPE.Rpc.Client
{
    public class DefaultClientMessageHandler : IClientMessageHandler
    {
        private readonly List<IMessageSubscriber> _subscribers;

        public DefaultClientMessageHandler(IEnumerable<IMessageSubscriber> subscribers)
        {
            _subscribers = subscribers.ToList();
        }

        public void RaiseReceive(AmpMessage message)
        {
            if (_subscribers?.Count > 0)
            {
                foreach (var subscriber in _subscribers)
                {
                    subscriber.Handle(message);
                }
            }
        }
    }
}
