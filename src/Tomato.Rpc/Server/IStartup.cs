using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Tomato.Rpc.Server
{
    public interface IStartup
    {
        void ConfigureServices(HostBuilderContext context,IServiceCollection services);
    }
}
