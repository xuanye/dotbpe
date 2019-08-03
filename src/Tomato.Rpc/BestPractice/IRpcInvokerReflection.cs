namespace Tomato.Rpc.BestPractice
{
    public interface IRpcInvokerReflection
    {
        IRpcInvoker GetRpcInvoker(int serviceId, ushort messageId);
    }
}
