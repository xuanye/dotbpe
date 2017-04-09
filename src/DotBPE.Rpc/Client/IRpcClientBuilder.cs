using System;
using System.Collections.Generic;
using System.Text;
using DotBPE.Rpc.Codes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotBPE.Rpc
{
    public interface IRpcClientBuilder
    {
        IRpcClient<TMessage> Build<TMessage>() where TMessage: InvokeMessage;

        IRpcClientBuilder UseLoggerFactory(ILoggerFactory loggerFactory);


        IRpcClientBuilder ConfigureServices(Action<IServiceCollection> configureServices);


        IRpcClientBuilder ConfigureLogging(Action<ILoggerFactory> configureLogging);

        IRpcClientBuilder UseSetting(string key, string value);


        string GetSetting(string key);
    }
}
