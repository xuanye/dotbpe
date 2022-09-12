// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using Peach;
using Peach.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Abstractions
{
    public interface IMessageHandler<TMessage> where TMessage : IMessage
    {
        Task ReceiveAsync(ISocketContext<TMessage> context, TMessage message);
    }

}
