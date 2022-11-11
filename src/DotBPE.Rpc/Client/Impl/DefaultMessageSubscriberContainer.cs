// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotBPE.Rpc.Client.Impl
{
    public class DefaultMessageSubscriberContainer : IMessageSubscriberContainer
    {
        private readonly List<IMessageSubscriber> _subscribers;

        public DefaultMessageSubscriberContainer(IEnumerable<IMessageSubscriber> subscribers)
        {
            _subscribers = subscribers.ToList();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriber"></param>
        public void Subscribe(IMessageSubscriber subscriber)
        {
            _subscribers.Add(subscriber);
        }

        public List<IMessageSubscriber> GetMessageSubscribers()
        {
            return _subscribers;
        }
    }
}
