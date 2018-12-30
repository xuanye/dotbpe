using Castle.DynamicProxy;
using DotBPE.Extra;
using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Client.RouterPolicy;
using DotBPE.Rpc.Config;
using DotBPE.Rpc.Protocol;
using DotBPE.Rpc.Server;
using MathService.Definition;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Peach;
using Peach.Protocol;
using System;
using System.Collections.Generic;

namespace Math.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            //TODO:简化调用情况
            IServiceCollection container = new ServiceCollection();
            container.AddSingleton<IProtocol<AmpMessage>, AmpProtocol>();
            container.AddSingleton<IProxyGenerator, ProxyGenerator>();
            container.AddSingleton<IClientProxy, DynamicClientProxy>();
            container.AddSingleton<IServiceRouter, DefaultServiceRouter>();
            container.AddSingleton<IServiceActorLocator<AmpMessage>, DefaultServiceActorLocator>();
            container.AddSingleton<ClientInterceptor>();
            container.AddSingleton<ICallInvoker, DefaultCallInvoker>();
            container.AddLogging(builder => builder.AddConsole());
            container.AddSingleton<IClientMessageHandler<AmpMessage>, DefaultClientMessageHandler>();
            container.AddSingleton<ISerializer, MessagePackSerializer>();
            container.AddSingleton<IRpcClient<AmpMessage>, DefaultRpcClient>();
            container.AddSingleton<ITransportFactory<AmpMessage>, DefaultTransportFactory>();
            container.AddSingleton<IRouterPolicy, RoundrobinPolicy>();
            container.Configure<RpcClientOptions>( x=> { });
            container.AddSingleton<ISocketClient<AmpMessage>, RpcSocketClient>();
            container.Configure<RouterPointOptions>(x => {
                x.Categories = new List<CategoryIdentifierOption>();
                x.Categories.Add(new CategoryIdentifierOption() { Category = "default", RemoteAddress = "10.128.1.105:5566" });
            });
            var services = container.BuildServiceProvider();
            var proxy = services.GetRequiredService<IClientProxy>();

            IMathService mathService = proxy.Create<IMathService>();
            var req = new SumReq() { A = 100, B = 101 };
            var result = mathService.SumAsync(req).Result;

            Console.WriteLine("Call Math Service ,return_code={0}",result.Code);
            if(result.Code == 0)
            {
                Console.WriteLine("Call Math Service Add {0}+{1}={2}", req.A, req.B, result?.Data?.Total);
            }
            Console.WriteLine("Press any key to exit !");
            Console.ReadKey();
            
        }
    }
}
