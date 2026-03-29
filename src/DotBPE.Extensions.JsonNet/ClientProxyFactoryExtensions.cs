using System;
using DotBPE.Rpc.Client;

namespace DotBPE.Extensions
{
    public static class ClientProxyFactoryExtensions
    {
        public static IClientProxyFactory UseJsonNetSerializer(this IClientProxyFactory @this)
        {
            return @this.AddDependencyServices(services => { services.AddJsonNetSerializer(); });

        }

    }
}

namespace DotBPE.Extra
{
    /// <summary>
    /// ClientProxyFactoryExtensions is obsolete. Use DotBPE.Extensions.ClientProxyFactoryExtensions instead.
    /// </summary>
    [Obsolete("Use DotBPE.Extensions.ClientProxyFactoryExtensions instead")]
    public static class ClientProxyFactoryExtensions
    {
        /// <summary>
        /// UseJsonNetSerializer is obsolete. Use DotBPE.Extensions.ClientProxyFactoryExtensions.UseJsonNetSerializer instead.
        /// </summary>
        [Obsolete("Use DotBPE.Extensions.ClientProxyFactoryExtensions.UseJsonNetSerializer instead")]
        public static IClientProxyFactory UseJsonNetSerializer(this IClientProxyFactory @this)
        {
            return DotBPE.Extensions.ClientProxyFactoryExtensions.UseJsonNetSerializer(@this);
        }
    }
}
