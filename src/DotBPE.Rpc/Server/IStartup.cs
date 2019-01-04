using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotBPE.Rpc.Server
{
    public interface IStartup
    {
        void ConfigureServices(HostBuilderContext context,IServiceCollection services);
    }
}
