using DotBPE.Rpc;
using System.Threading.Tasks;

namespace DotBPE.Gateway
{
    public delegate Task<RpcResult<TResponse>> RpcServiceMethod<in TService, in TRequest, TResponse>(TService service, TRequest request);

    public delegate Task<RpcResult<TResponse>> RpcServiceMethodWithTimeout<in TService, in TRequest, TResponse>(TService service, TRequest request,int timeout);
}
