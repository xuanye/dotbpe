namespace DotBPE.Rpc
{
    public interface IClientProxy
    {
        TClient GetClient<TClient>() where TClient : class, IInvokeClient;

        TService GetService<TService>();
    }
}
