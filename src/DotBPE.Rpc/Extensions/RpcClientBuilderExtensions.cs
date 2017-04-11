using DotBPE.Rpc.Client;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.DefaultImpls;
using System;
using DotBPE.Rpc.Exceptions;
using DotBPE.Rpc.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace DotBPE.Rpc.Extensions
{
    public static class RpcClientBuilderExtensions
    {
        public static IRpcClientBuilder AddCore<TMessage>(this IRpcClientBuilder builder) where TMessage:InvokeMessage
        {
            
            builder.ConfigureServices((services) =>
            {
                services.AddSingleton<ITransportFactory<TMessage>,DefaultTransportFactory<TMessage>>()
                    .AddSingleton<IMessageHandler<TMessage>>(new ClientMessageHandler<TMessage>())
                .AddSingleton<IRpcClient<TMessage>,DefaultRpcClient<TMessage>>();
            });
            return builder;
        }
        public static IRpcClientBuilder UseServer(this IRpcClientBuilder builder ,string remoteAddress)
        {
            Preconditions.CheckArgument(!string.IsNullOrEmpty(remoteAddress), "服务器地址不能为空");
            string[] arr_address = remoteAddress.Split(':', ',');
            if(arr_address.Length != 2)
            {
                throw new ArgumentOutOfRangeException("remoteAddress", "参数格式不正确,正确的格式应为(ip:port),不包含括号");
            }
            builder.UseSetting("serverIP", arr_address[0]);
            builder.UseSetting("serverPort", arr_address[1]);
            return builder;
        }
    }
}
