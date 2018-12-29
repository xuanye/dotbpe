using Peach.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Peach.EventArgs
{
    public class MessageReceivedEventArgs<TMessage> where TMessage :IMessage
    {
        public MessageReceivedEventArgs()
        {
        }

        public MessageReceivedEventArgs(ISocketContext<TMessage> context, TMessage message)
        {
            this.Context = context;
            this.Message = message;
        }

        public ISocketContext<TMessage> Context { get; set; }
        public TMessage Message { get; set; }
    }
}
