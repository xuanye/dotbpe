namespace DotBPE.Rpc.Server
{
    public class RpcServiceOptions
    {
        /// <summary>
        /// Get a collection of interceptors to be executed with every call. Interceptors are executed in order.
        /// </summary>
        public InterceptorCollection Interceptors { get; } = new InterceptorCollection ();

    }
}
