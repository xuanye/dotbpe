using System;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotBPE.Rpc.Hosting;
using DotBPE.Rpc.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace DotBPE.Rpc.DefaultImpls
{
    public class DefaultServerHost : IServerHost
    {
        private readonly IServerBootstrap _bootstrap;
        static readonly ILogger Logger = Environment.Logger.ForType<DefaultServerHost>();
        private readonly IServiceProvider provider;

        private readonly RpcHostOption _option;
        public DefaultServerHost(IServerBootstrap bootstrap, IServiceProvider provider, RpcHostOption option)
        {
            this.provider = provider;
            this._bootstrap = bootstrap;

            this._option = option;
        }

        public Task ShutdownAsync()
        {
            return this._bootstrap.ShutdownAsync();
        }

        public Task StartAsync()
        {
            Logger.Debug($"server host at {_option.HostIP}:{_option.HostPort} ...");
            var endpoint = new IPEndPoint(IPAddress.Parse(_option.HostIP), _option.HostPort);
            return this._bootstrap.StartAsync(endpoint);
        }

        public Task Preheating()
        {
            var preTasks = this.provider.GetServices<IPreheating>();
            if(preTasks !=null)
            {
                var  listT = new List<Task>();
                foreach(IPreheating task in preTasks){
                    listT.Add(task.StartAsync());
                }
                return Task.WhenAll(listT);
            }
            return Task.CompletedTask;
        }
    }
}
