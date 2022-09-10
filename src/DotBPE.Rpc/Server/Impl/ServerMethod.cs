using System.Threading.Tasks;
using DotBPE.Rpc.Protocol;
using Peach;

namespace DotBPE.Rpc.Server
{
    /// <summary>
    /// Server-side handler for a unary call.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public delegate Task<TResponse> ServerMethod<TService,TRequest,TResponse>(TService service,
        TRequest request,
        ISocketContext<AmpMessage> context);
}
