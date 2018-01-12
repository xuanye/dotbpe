using System.Net;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Hosting
{
    public interface IServerBootstrap
    {
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="endpoint">需要绑定的IP</param>
        /// <returns></returns>
        Task StartAsync(EndPoint endpoint);

        Task ShutdownAsync();
    }
}
