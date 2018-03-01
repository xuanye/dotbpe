using System.Net;
using System.Threading.Tasks;

namespace DotBPE.Rpc
{

    public interface IServerBootstrap
    {
        /// <summary>
        /// Start Server async.
        /// </summary>
        /// <param name="localPoint">绑定到本地的服务地址</param>
        /// <returns></returns>
        Task StartAsync(EndPoint localPoint);

        /// <summary>
        /// Shutdown Server
        /// </summary>
        /// <returns></returns>
        Task ShutdownAsync();
    }


}
