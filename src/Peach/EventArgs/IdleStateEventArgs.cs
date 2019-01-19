using Peach.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Peach.EventArgs
{    
    public class IdleStateEventArgs<TMessage> where TMessage : IMessage
    {
        public IdleStateEventArgs()
        {
        }

        public IdleStateEventArgs(ISocketContext<TMessage> context)
        {
            Context = context;
        }

        public ISocketContext<TMessage> Context { get; set; }

    }
}
