using DotBPE.Rpc.Codes;

namespace DotBPE.Rpc {
    public class MessageRecievedEventArgs<TMessage> where TMessage : InvokeMessage {
        public MessageRecievedEventArgs () { }

        public MessageRecievedEventArgs (IRpcContext<TMessage> context, TMessage message) {
            this.Context = context;
            this.Message = message;
        }

        public IRpcContext<TMessage> Context { get; set; }
        public TMessage Message { get; set; }
    }
}