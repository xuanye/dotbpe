// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Client
{
    public interface IMessageSubscriberContainer
    {
        List<IMessageSubscriber> GetMessageSubscribers();

    }
}
