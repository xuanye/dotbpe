using System;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Hosting
{
    public interface IServerHost
    {
        Task StartAsync();

        Task ShutdownAsync();
    }
}
