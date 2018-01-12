using DotBPE.Rpc.Codes;

namespace DotBPE.Rpc
{
    /// <summary>
    /// 服务定位器，实现者需要根据message中的内容，准确的找到具体的IServiceActor
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface IServiceActorLocator<TMessage> where TMessage : IMessage
    {
        IServiceActor<TMessage> LocateServiceActor(TMessage message);
    }
}
