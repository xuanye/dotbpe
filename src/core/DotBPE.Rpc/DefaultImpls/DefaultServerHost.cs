using DotBPE.Rpc.Hosting;
using DotBPE.Rpc.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DotBPE.Rpc.DefaultImpls
{
    public class DefaultServerHost : IServerHost
    {
        private static readonly ILogger Logger = Environment.Logger.ForType<DefaultServerHost>();

        private IServiceProvider _applicationServices;
        private IStartup _startup;
        private bool _initialized;

        private readonly IServiceCollection _applicationServiceCollection;
        private readonly IServiceProvider _hostProvider;
        private readonly RpcHostOption _option;
        private IServerBootstrap _server;

        public DefaultServerHost(IServiceProvider hostProvider, IServiceCollection serviceCollection, RpcHostOption option)
        {
            this._hostProvider = hostProvider;
            this._applicationServiceCollection = serviceCollection;
            this._option = option;
        }

        public Task ShutdownAsync()
        {
            return this._server.ShutdownAsync();
        }

        public async Task StartAsync()
        {
            Initialize();
            var endpoint = new IPEndPoint(IPAddress.Parse(_option.HostIP), _option.HostPort);
            await this._server.StartAsync(endpoint);
            Logger.Debug($"server host at {_option.HostIP}:{_option.HostPort} ...");
        }

        public Task Preheating()
        {
            var preTasks = this._applicationServices.GetServices<IPreheating>();
            if (preTasks != null)
            {
                var listT = new List<Task>();
                foreach (IPreheating task in preTasks)
                {
                    listT.Add(task.StartAsync());
                }
                return Task.WhenAll(listT);
            }
            return Task.CompletedTask;
        }

        public void Initialize()
        {
            if (!_initialized)
            {
                BuildApplication();
            }
        }

        private void BuildApplication()
        {
            //注册所有的依赖和服务
            EnsureApplicationServices();
            EnsureServer();

            //_applicationServices.GetRequiredService<IServiceActorContainer>();
            var appbuilder = _applicationServices.GetRequiredService<IAppBuilder>();
            var env = _applicationServices.GetRequiredService<IHostingEnvironment>();
            appbuilder.ServiceProvider = this._applicationServices;

            _startup.Configure(appbuilder, env);

            Environment.SetServiceProvider(this._applicationServices);
            _initialized = true;
        }

        private void EnsureApplicationServices()
        {
            if (_applicationServices == null)
            {
                EnsureStartup();
                _applicationServices = _startup.ConfigureServices(this._applicationServiceCollection);
            }
        }

        private void EnsureServer()
        {
            if (_server == null)
            {
                _server = _applicationServices.GetRequiredService<IServerBootstrap>();
            }
        }

        private void EnsureStartup()
        {
            if (_startup != null)
            {
                return;
            }

            _startup = _hostProvider.GetRequiredService<IStartup>();
        }
    }
}
