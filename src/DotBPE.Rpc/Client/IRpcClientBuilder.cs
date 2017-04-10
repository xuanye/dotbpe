using System;
using System.Collections.Generic;
using System.Text;
using DotBPE.Rpc.Codes;
using Microsoft.Extensions.DependencyInjection;


namespace DotBPE.Rpc
{
    public interface IRpcClientBuilder
    {
        IRpcClient<TMessage> Build<TMessage>() where TMessage: InvokeMessage;

       


        IRpcClientBuilder ConfigureServices(Action<IServiceCollection> configureServices);


    

        IRpcClientBuilder UseSetting(string key, string value);


        string GetSetting(string key);
    }
}
