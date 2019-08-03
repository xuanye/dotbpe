using Tomato.Rpc.Client;

namespace Tomato.Extra
{
    public static class ClientProxyFactoryExtensions
    {
        public static IClientProxyFactory UseJsonNetSerializer(this IClientProxyFactory @this)
        {
            return @this.AddDependencyServices(services => { services.AddJsonNetSerializer(); });

        }

    }
}
