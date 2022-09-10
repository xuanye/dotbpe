namespace DotBPE.Rpc.Server
{
    public interface IRpcServiceMethodProvider<TService> where TService : class
    {
        void OnServiceMethodDiscovery(RpcServiceMethodProviderContext<TService> context);
    }
}
