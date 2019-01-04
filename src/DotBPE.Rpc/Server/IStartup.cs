using Microsoft.Extensions.DependencyInjection;

namespace DotBPE.Rpc.Server
{
    public interface IStartup
    {
        void ConfigureServices(IServiceCollection service);
    }
}
