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
            DotBPE.Rpc.Environment.SetLogger(new DotBPE.Rpc.Logging.ConsoleLogger());

            var client = DotBpeAmpClient.Create("127.0.0.1:6201");
            var greeter = new GreeterClient(client);
            while (true)
            {

                string name = "xuanye";
                if ("bye".Equals(name))
                {
                    break;
                }
                var reply = greeter.HelloPlusAsnyc(new HelloRequest()
                {
                    Name = name
                },600000).Result;

                DotBPE.Rpc.Environment.Logger.Debug($"---------------收到服务端返回:{reply.Message}-----------");
            }
        }
    }


}