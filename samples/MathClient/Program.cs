// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Extra;
using DotBPE.Rpc.Client;
using MathService.Definition;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Math.Client
{
    static class Program
    {
        static void Main(string[] args)
        {

            /* default route  */
            var factory = ClientProxyFactory.Create()
                .UseCastleDynamicClientProxy()
                .ConfigureLogging(logger =>
                {
                    logger.SetMinimumLevel(LogLevel.Debug);
                    logger.AddConsole();
                })
                .UseMessagePackSerializer()
                .UseDefaultChannel($"127.0.0.1:5566");



            /* service discovery route
            var factory = ClientProxyFactory.Create()
                .ConfigureLogging(logger =>logger.AddConsole())
                .UseCastleDynamicProxy()
                .UseMessagePackSerializer()
                .UseConsulDnsServiceDiscovery(); //默认配置 localhost
                */

            var proxy = factory.GetClientProxy();

            var mathService = proxy.Create<IMathService>();

            var i = 0;
            var random = new Random();
            while (i++ < 100)
            {
                var req = new SumReq { A = random.Next(100000), B = random.Next(100000) };
                var result = mathService.SumAsync(req).GetAwaiter().GetResult();

                Console.WriteLine("Call Math Service ,return_code={0}", result.Code);
                if (result.Code == 0)
                {
                    Console.WriteLine("Call Math Service Add {0}+{1}={2}", req.A, req.B, result.Data?.Total);
                }

                Console.WriteLine("============= count:{0}", i);           
              
            }

            Console.WriteLine("Press any key to exit !");
            var key =  Console.ReadKey();
            if (key.Key == ConsoleKey.C)
            { 
                Console.WriteLine("=====recall======== ");   
                var req = new SumReq { A = random.Next(100000), B = random.Next(100000) };
                var result = mathService.SumAsync(req).GetAwaiter().GetResult();

                Console.WriteLine("Call Math Service ,return_code={0}", result.Code);
                if (result.Code == 0)
                {
                    Console.WriteLine("Call Math Service Add {0}+{1}={2}", req.A, req.B, result.Data?.Total);
                }

                Console.WriteLine("============= count:{0}", i);           
            }
            
            
        }
    }
}
