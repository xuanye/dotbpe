using Peach.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Peach.EventArgs
{   
    public class DisconnectedEventArgs<TMessage> where TMessage : IMessage
    {
        public DisconnectedEventArgs()
        {
        }

        public DisconnectedEventArgs(ISocketContext<TMessage> context)
        {
            Context = context;
        }

        public ISocketContext<TMessage> Context { get; set; }

    }
}
