using System.Threading.Tasks;
using DotBPE.Rpc.Server;

namespace DotBPE.Rpc
{
    public delegate Task RpcRequestDelegate(IRpcContext context);
}
