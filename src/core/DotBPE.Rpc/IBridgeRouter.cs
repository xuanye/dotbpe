using DotBPE.Rpc.Codes;

namespace DotBPE.Rpc
{
    public interface IBridgeRouter<TMessage> where TMessage : InvokeMessage
    {
        RouterPoint GetRouterPoint(TMessage message);
    }
}
