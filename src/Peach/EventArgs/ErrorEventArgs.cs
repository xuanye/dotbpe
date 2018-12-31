using Peach.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Peach.EventArgs
{
    public class ErrorEventArgs<TMessage> where TMessage:IMessage
    {
        public ErrorEventArgs()
        {
        }

        public ErrorEventArgs(ISocketContext<TMessage> context, Exception ex)
        {
            Context = context;
            Error = ex;
        }

        public ISocketContext<TMessage> Context { get; set; }
        public Exception Error { get; set; }
    }
}
