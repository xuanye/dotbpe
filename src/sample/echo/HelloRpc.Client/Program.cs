using System;
using System.Text;
using System.Threading.Tasks;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Extensions;
using DotBPE.Rpc.Netty;
using Microsoft.Extensions.DependencyInjection;
using HelloRpc.Common;

namespace HelloRpc.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            var client = DotBpeAmpClient.Create("127.0.0.1:6201");
            var greeter = new GreeterClient(client);
            while (true)
            {
                Console.WriteLine("-------------请输入你的名字-------------");
                string name = Console.ReadLine();
                if ("bye".Equals(name))
                {
                    break;
                }
                var reply = greeter.SayHelloAsnyc(new HelloRequest()
                {
                    Name = name
                }).Result;
              
                Console.WriteLine($"---------------收到服务端返回:{reply.Message}-----------");
            }
        }      
    }


}