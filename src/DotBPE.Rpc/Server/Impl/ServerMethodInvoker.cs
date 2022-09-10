namespace DotBPE.Rpc.Server
{
    internal class ServerMethodInvoker<TService, TRequest, TResponse>
        where TRequest : class
        where TResponse : class
        where TService : class
    {
        private readonly ServerMethod<TService, TRequest, TResponse> _invoker;

        public ServerMethodInvoker(
            ServerMethod<TService, TRequest, TResponse> invoker,
            Method<TRequest, TResponse> method,
            MethodOptions options)
        {
            _invoker = invoker;
        }
    }
}
