using DotBPE.Extra;
using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Config;
using MathService.Definition;
using Microsoft.Extensions.Logging;
using System;
using Peach.Infrastructure;

namespace Math.Client
{
    static class Program
    {
        static void Main(string[] args)
        {

            /* default route  */
            var factory = ClientProxyFactory.Create()
                .UseCastleDynamicProxy()
                .ConfigureLogging(logger =>logger.AddConsole())
                .UseMessagePack()
                .UseDefaultChannel($"{IPUtility.GetLocalIntranetIP().MapToIPv4()}:5566");



            /* Service Discovery
            var factory = ClientProxyFactory.Create()
                .ConfigureLogging(logger =>logger.AddConsole())
                .UseCastleDynamicProxy()
                .UseMessagePack()
                .UseConsulDnsServiceDiscovery(); //默认配置 localhost
                */

            var proxy = factory.GetProxyInstance();

            var mathService = proxy.Create<IMathService>();

            var i = 0;
            var random = new Random();
            while (i++ < 100)
            {
                var req = new SumReq {A = random.Next(100000), B = random.Next(100000)};
                var result = mathService.SumAsync(req).Result;

                Console.WriteLine("Call Math Service ,return_code={0}", result.Code);
                if (result.Code == 0)
                {
                    Console.WriteLine("Call Math Service Add {0}+{1}={2}", req.A, req.B, result.Data?.Total);
                }

                Console.WriteLine("============= count:{0}",i);
            }

            Console.WriteLine("Press any key to exit !");
            Console.ReadKey();
        }
    }
}
