using DotBPE.Rpc;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Extensions;
using DotBPE.Rpc.Netty;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Protocol.Amp
{
    public class DotBpeAmpClient
    {
        public static IRpcClient<AmpMessage> Create(string remoteAddress)
        {
            // 生成Client的部分 可以在DotBPE.Protocol.Amp 进一步封装
            var client = new RpcClientBuilder()
                .AddCore<AmpMessage>()
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
