using Consul;
using DotBPE.Baseline.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DotBPE.Rpc;
using DotBPE.Extra;
using DotBPE.Rpc.Internal;

namespace MathService
{
    static class Program
    {
        static void Main(string[] args)
        {
            /* */
            var builder = new HostBuilder()
             .UseRpcServer()
             .UseCastleDynamicProxy()
             .UseMessagePackSerializer()
             .BindService<Definition.MathService>()
             //.BindServices(services => { services.Add<Definition.MathService>();})
             .ConfigureLogging(
                 logger =>
                 {
                     logger.AddConsole();
                 }
             );

            //启动
            builder.RunConsoleAsync().Wait();


            /*
             * Consul Service Registration
             *
             var builder = new HostBuilder()
             .UseRpcServer()
             .UseCastleDynamicProxy()
             .UseMessagePack()
             .BindService<Definition.MathService>()
             //.BindServices(services => { services.Add<Definition.MathService>();})
             .UseConsulServiceRegistration((point, list) =>
             {
                 var tcpCheck = new AgentServiceCheck
                 {
                     DeregisterCriticalServiceAfter = 1.Minutes(),
                     Interval = 30.Seconds(),
                     TCP = EndPointParser.ParseEndPointToString(point.RemoteAddress)
                 };
                 list.Add(tcpCheck);
             }) //添加服务注册的依赖
             .ConfigureLogging(
                 logger =>
                 {
                     logger.AddConsole();
                 }
             );

            //启动
            builder.RegisterAndRunConsoleAsync().Wait();
            */


        }
    }
}
