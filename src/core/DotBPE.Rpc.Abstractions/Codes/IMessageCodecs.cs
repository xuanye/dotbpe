namespace DotBPE.Rpc.Codes
{
    public interface IMessageCodecs<TMessage> where TMessage : InvokeMessage
    {
        void Encode(TMessage message, IBufferWriter writer);

        TMessage Decode(IBufferReader reader);

        MessageMeta GetMessageMeta();

        TMessage HeartbeatMessage();
    }    
}
