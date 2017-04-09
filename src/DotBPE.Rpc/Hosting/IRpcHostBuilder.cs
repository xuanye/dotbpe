using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotBPE.Rpc.Hosting
{
    public interface IRpcHostBuilder
    {
        IServerHost Build();
      
        IRpcHostBuilder UseLoggerFactory(ILoggerFactory loggerFactory);


        IRpcHostBuilder ConfigureServices(Action<IServiceCollection> configureServices);

      
        IRpcHostBuilder ConfigureLogging(Action<ILoggerFactory> configureLogging);

        IRpcHostBuilder UseSetting(string key, string value);

       
        string GetSetting(string key);
    }
}
