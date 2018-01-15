using DotBPE.Hosting;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;
using System.Threading.Tasks;

namespace HelloServer
{
    class Program
    {
        public static void Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    //添加服务端核心的依赖和协议支持
                    services.AddDotBPE();
                  
                    //添加挂载的宿主服务
                    services.AddScoped<IHostedService, RpcHostedService>();                   
                });

            builder.RunConsoleAsync().Wait();          
        }
    }
    
   
}
