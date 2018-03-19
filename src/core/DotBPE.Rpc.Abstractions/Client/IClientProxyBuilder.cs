using System;
using Microsoft.Extensions.DependencyInjection;

namespace DotBPE.Rpc {
    public interface IClientProxyBuilder {
        IClientProxy Build ();

        IClientProxyBuilder ConfigureServices (Action<IServiceCollection> configureServices);

        IClientProxyBuilder UseSetting (string key, string value);

        string GetSetting (string key);
    }
}