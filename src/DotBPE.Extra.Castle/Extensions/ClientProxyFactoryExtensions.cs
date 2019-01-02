using DotBPE.Rpc.Client;

namespace DotBPE.Extra
{
    public static class ClientProxyFactoryExtensions
    {
        public static IClientProxyFactory UseCastleDynamicProxy(this IClientProxyFactory @this)
        {
            return @this.AddDependencyServices(services => { services.AddDynamicClientProxy(); });
        }

    }
}
