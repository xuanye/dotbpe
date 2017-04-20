using DotBPE.Rpc;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Extensions;
using DotBPE.Rpc.Netty;
using Microsoft.Extensions.DependencyInjection;

namespace DotBPE.Protocol.Amp
{
    public class AmpClient
    {
        public static IRpcClient<AmpMessage> Create(string remoteAddress,int multiplexCount=5)
        {
            // 生成Client的部分 可以在DotBPE.Protocol.Amp 进一步封装
            var client = new RpcClientBuilder()
                .AddCore<AmpMessage>()
                .UseSetting("MultiplexCount","1")
                .UserNettyClient<AmpMessage>()
                .ConfigureServices((services) =>
                {
                    services.AddSingleton<IMessageCodecs<AmpMessage>, AmpCodecs>();
                })
                .UseServer(remoteAddress)
                .Build<AmpMessage>();

            return client;
        }
    }
}
