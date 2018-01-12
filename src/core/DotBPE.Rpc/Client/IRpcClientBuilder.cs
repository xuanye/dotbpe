using DotBPE.Rpc.Codes;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DotBPE.Rpc
{
    public interface IRpcClientBuilder
    {
        IRpcClient<TMessage> Build<TMessage>() where TMessage : InvokeMessage;

        IRpcClientBuilder ConfigureServices(Action<IServiceCollection> configureServices);

        IRpcClientBuilder UseSetting(string key, string value);

        string GetSetting(string key);
    }
}
