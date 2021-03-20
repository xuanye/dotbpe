using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Config;
using DotBPE.Rpc.Internal;
using DotBPE.Rpc.Protocol;
using DotBPE.Rpc.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Peach.Config;

namespace DotBPE.Rpc.ServiceDiscovery
{
    /// <inheritdoc />
    public class DefaultServiceRegister:IServiceRegister
    {
        private readonly IServiceRegistrationProvider _registrationProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DefaultServiceRegister> _logger;
        private readonly RpcServerOptions _optionValues;
        private readonly ConcurrentDictionary<string,IRouterPoint> CACHE_POINT = new ConcurrentDictionary<string, IRouterPoint>();

        private readonly IPEndPoint _localBindAddress;
        public  DefaultServiceRegister(
            IServiceRegistrationProvider registrationProvider,
            IServiceProvider serviceProvider,
            IOptions<RpcServerOptions> options,ILogger<DefaultServiceRegister> logger)
        {
            this._registrationProvider = registrationProvider;
            this._serviceProvider = serviceProvider;
            this._logger = logger;
            this._optionValues = options.Value;

            this._localBindAddress = new IPEndPoint(GetBindAddress(),this._optionValues.Port);
        }


        public async Task RegisterAllServices()
        {
           var actors = this._serviceProvider.GetServices<IServiceActor<AmpMessage>>();

           if (actors != null && actors.Any())
           {
               foreach (var actor in actors)
               {
                   //默认以AppName+groupName作为服务标识
                   string key = GetServiceName(this._optionValues, actor);
                   var point = new RouterPoint
                   {
                       RoutePointType = RoutePointType.Remote,
                       RemoteAddress = this._localBindAddress
                   };
                   CACHE_POINT.AddOrUpdate(key, point, (a,b) => point);
               }
           }

           if (!this.CACHE_POINT.IsEmpty)
           {
               foreach (var kv in this.CACHE_POINT)
               {
                   var id = kv.Key+"@"+ EndPointParser.ParseEndPointToString(kv.Value.RemoteAddress);
                   await this._registrationProvider.RegisterServiceAsync(id,kv.Key,kv.Value);
               }
           }
           this._logger.LogInformation("register service completed ,service count ={0}",this.CACHE_POINT.Count);
        }

        public async Task DeregisterAllServices()
        {
            if (!this.CACHE_POINT.IsEmpty)
            {
                foreach (var kv in this.CACHE_POINT)
                {
                    var id = kv.Key+"@"+ EndPointParser.ParseEndPointToString(kv.Value.RemoteAddress);
                    await this._registrationProvider.DeregisterServiceAsync(id);
                }
            }
            this._logger.LogInformation("deregister service completed ,service count ={0}",this.CACHE_POINT.Count);
        }

        private IPAddress GetBindAddress()
        {
            var address = IPAddress.Parse("127.0.0.1");

            switch (_optionValues.BindType)
            {
                case AddressBindType.SpecialAddress:
                    address = IPAddress.Parse(_optionValues.SpecialAddress);
                    break;
                case  AddressBindType.InternalAddress:
                    address = Peach.Infrastructure.IPUtility.GetLocalIntranetIP();
                    break;
            }
            return address;
        }

        protected virtual string GetServiceName(RpcServerOptions optionValue,IServiceActor<AmpMessage> actor)
        {
            return $"{optionValue.AppName}-{actor.GroupName}";
        }
    }
}
