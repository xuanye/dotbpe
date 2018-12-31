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
using Peach.Infrastructure;

namespace Math.Client
{
    static class Program
    {
        static void Main(string[] args)
        {

            var factory = ClientProxyFactory.Create()
                .UseCastleDynamicProxy()
                .UseMessagePack()
                .UseDefaultChannel($"{IPUtility.GetLocalIntranetIP().MapToIPv4()}:5566");

            var proxy = factory.GetProxyInstance();

            var mathService = proxy.Create<IMathService>();

            var i = 1;
            var random = new Random();
            while (i++ < 100)
            {
                var req = new SumReq {A = random.Next(100000), B = random.Next(100000)};
                var result = mathService.SumAsync(req).Result;

                Console.WriteLine("Call Math Service ,return_code={0}", result.Code);
                if (result.Code == 0)
                {
                    //Console.WriteLine("Call Math Service Add {0}+{1}={2}", req.A, req.B, result?.Data?.Total);
                }

                Console.WriteLine("============= count:{0}",i);
            }

            Console.WriteLine("Press any key to exit !");
            Console.ReadKey();
        }
    }
}
