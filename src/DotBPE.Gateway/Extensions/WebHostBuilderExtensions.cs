using DotBPE.Rpc.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Peach;
using DotBPE.Rpc.Config;
using DotBPE.Rpc.Server;
using Peach.Config;
using Microsoft.AspNetCore.Hosting;

namespace DotBPE.Gateway
{
    public static class WebHostBuilderExtensions
    {

       /// <summary>
       /// 在HTTP服务上 同时挂载Rpc服务，一般用于测试
       /// </summary>
       /// <param name="builder"></param>
       /// <param name="appName"></param>
       /// <param name="port"></param>
       /// <param name="bindType"></param>
       /// <param name="specialAddress"></param>
       /// <returns></returns>
        public static IWebHostBuilder UseRpcServer(this IWebHostBuilder builder, string appName = "dotbpe", int port = 5566,
          AddressBindType bindType = AddressBindType.InternalAddress, string specialAddress = null)
        {
            return builder.ConfigureServices(services =>
            {
                services.Configure<RpcServerOptions>(o =>
                {
                    o.Port = port;
                    o.BindType = bindType;
                    o.AppName = appName;
                    o.BindType = bindType;
                    //special address logical
                    if (string.IsNullOrEmpty(specialAddress)) return;
                    o.BindType = AddressBindType.SpecialAddress;
                    o.SpecialAddress = specialAddress;
                });
                services.AddHostedService<RpcHostedService>();
                services.AddSingleton<IServerBootstrap, AmpTcpServerBootstrap>();
            });
        }
    }
}
