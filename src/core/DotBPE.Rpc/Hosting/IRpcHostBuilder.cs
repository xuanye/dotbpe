using Microsoft.Extensions.DependencyInjection;
using System;

namespace DotBPE.Rpc.Hosting
{
    public interface IRpcHostBuilder
    {
        IServerHost Build();

        IRpcHostBuilder ConfigureServices(Action<IServiceCollection> configureServices);

        IRpcHostBuilder UseSetting(string key, string value);

        string GetSetting(string key);
    }
}
