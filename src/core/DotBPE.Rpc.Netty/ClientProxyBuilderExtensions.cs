namespace DotBPE.Rpc.Netty
{
    public static class ClientProxyBuilderExtensions
    {
        public static IClientProxyBuilder AddNettyClient<TMessage>(this IClientProxyBuilder builder) where TMessage : InvokeMessage
        {
            builder.ConfigureServices((services) =>
            {
                services.AddNettyClient<TMessage>();
            });
            return builder;
        }
    }
}
