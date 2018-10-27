namespace DotBPE.Rpc
{
    public interface IRouter<TMessage> where TMessage : InvokeMessage
    {
        RouterPoint GetRouterPoint(TMessage message);
    }
}
