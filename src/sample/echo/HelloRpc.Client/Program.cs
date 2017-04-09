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
            var actor = new DefaultClientActor();
            actor.Recieved += Actor_Recieved;
            var client = new RpcClientBuilder()
                .AddCore<AmpMessage>()
                .UserNettyClient<AmpMessage>()
                .ConfigureServices((services) =>
                {
                    services.AddSingleton<IServiceActor<AmpMessage>>(actor)
                    .AddSingleton<IServiceActorLocator<AmpMessage>,ClientActorLocator>()
                    .AddSingleton<IMessageCodecs<AmpMessage>, AmpCodecs>();
                })
                .UseSetting("serverIp","127.0.0.1")
                .UseSetting("serverPort","6201")
                .Build<AmpMessage>();

            Console.WriteLine("Echo Client Starting");
            client.SendString("Hello");
            Console.ReadLine();
        }

        private static void Actor_Recieved(object sender, MessageRecievedEventArgs<AmpMessage> e)
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