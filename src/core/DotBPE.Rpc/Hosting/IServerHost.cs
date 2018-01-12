using System.Threading.Tasks;

namespace DotBPE.Rpc.Hosting
{
    public interface IServerHost
    {
        Task StartAsync();

        /// <summary>
        /// 预热，初始化工作
        /// </summary>
        /// <returns></returns>
        Task Preheating();

        Task ShutdownAsync();

        void Initialize();
    }
}
