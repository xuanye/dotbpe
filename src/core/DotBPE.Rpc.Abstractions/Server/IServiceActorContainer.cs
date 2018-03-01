using DotBPE.Rpc.Codes;

namespace DotBPE.Rpc
{
    public interface IServiceActorContainer<TMessage> where TMessage : InvokeMessage
    {
        IServiceActor<TMessage> GetById(string Id);
    }
}
