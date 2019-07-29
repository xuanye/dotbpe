using System;
using System.Threading.Tasks;
using Tomato.Rpc.Config;
using Tomato.Rpc.ServiceDiscovery;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Tomato.Rpc.Client
{
    /// <summary>
    /// 基于服务注册和发现的服务路由
    /// </summary>
    public class DiscoveryServiceRouter : IServiceRouter
    {
        private readonly IServiceDiscoveryProvider _discoveryProvider;


        private readonly string _appName;
        public DiscoveryServiceRouter(IServiceDiscoveryProvider discoveryProvider,IServiceProvider provider)
        {
            this._discoveryProvider = discoveryProvider;

            var clientOptions = provider.GetService<IOptions<RpcClientOptions>>();

            this._appName = clientOptions?.Value != null ?
                clientOptions.Value.AppName : new RpcClientOptions().AppName;

        }

        public Task<IRouterPoint> FindRouterPoint(string servicePath)
        {

            var serviceIdentity = ConvertServiceIdentityFromServicePath(servicePath);
            //TODO: cache it?
            return _discoveryProvider.LookupServiceDefaultEndPointAsync(serviceIdentity);
        }

        /// <summary>
        /// 通过服务路径来获取服务标识，默认为服务分组标识
        /// </summary>
        /// <param name="servicePath"></param>
        /// <returns></returns>
        protected virtual string ConvertServiceIdentityFromServicePath(string servicePath)
        {
            string[] parts = servicePath.Split('.');

            return $"{this._appName}-{parts[0]}";
        }
    }
}
