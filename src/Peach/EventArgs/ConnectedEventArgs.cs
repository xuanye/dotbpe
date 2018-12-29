
using Peach.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Peach.EventArgs
{   
    public class ConnectedEventArgs<TMessage> where TMessage : IMessage
    {
        public ConnectedEventArgs()
        {
        }

        public ConnectedEventArgs(ISocketContext<TMessage> context)
        {
            this.Context = context;           
        }

        public ISocketContext<TMessage> Context { get; set; }
       
    }
}
