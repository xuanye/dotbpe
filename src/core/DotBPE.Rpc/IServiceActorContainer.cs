using DotBPE.Rpc.Codes;

namespace DotBPE.Rpc
{
    public interface IServiceActorContainer<TMessage> where TMessage :InvokeMessage
    {
        //void Initialize(IServiceProvider provider);
        IServiceActor<TMessage> GetById(string Id);
    }
}
