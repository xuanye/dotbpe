using System;
using System.Text;
using System.Threading.Tasks;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Extensions;
using DotBPE.Rpc.Netty;
using Microsoft.Extensions.DependencyInjection;
namespace HelloRpc.Client
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.OutputEncoding = Encoding.UTF8;
           
            var client = new RpcClientBuilder()
                .AddCore<AmpMessage>()
                .UserNettyClient<AmpMessage>()
                .ConfigureServices((services) =>
                {
                    services.AddSingleton<IMessageCodecs<AmpMessage>, AmpCodecs>();
                })
                .UseSetting("serverIp","127.0.0.1")
                .UseSetting("serverPort","6201")
                .Build<AmpMessage>();

            client.Recieved += Client_Recieved;
            Console.WriteLine("Echo Client Starting");
            client.SendString("Hello");
            Console.ReadLine();
        }

        private static void Client_Recieved(object sender, MessageRecievedEventArgs<AmpMessage> e)
        {
            e.Context.SendAsync(e.Message);
            Console.WriteLine($"[Client]:Message Receieved:{e.Message.ServiceId}:{e.Message.MessageId}");
        }      
    }

    public static class RpcClientExtensions
    {
        public static Task SendString(this IRpcClient<AmpMessage> client, string msg)
        {
            AmpMessage request = new AmpMessage()
            {
                InvokeMessageType = InvokeMessageType.Request,
                Data = Encoding.UTF8.GetBytes(msg),
                ServiceId = 100,
                MessageId = 1
            };

            return client.SendAsync(request);
        }
    }
}