using System.Collections.Generic;

namespace DotBPE.Rpc.Server
{
    public class RpcServiceMethodProviderContext<TService> where TService : class
    {

        public RpcServiceMethodProviderContext()
        {
            Methods = new List<RpcMethodModel>();
        }

        internal List<RpcMethodModel> Methods { get; }

        /// <summary>
        /// Add method
        /// </summary>
        /// <param name="method"></param>
        /// <param name="methodKey"></param>
        /// <param name="metadata"></param>
        /// <param name="invoker"></param>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        public void AddMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, string methodKey, IList<object> metadata, RpcRequestDelegate invoker)
            where TRequest : class
            where TResponse : class
        {
            var methodModel = new RpcMethodModel(method, methodKey, metadata, invoker);
            Methods.Add(methodModel);
        }
    }
}
