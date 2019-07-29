using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Consul;
using Tomato.Rpc.Client;
using Tomato.Rpc.Internal;
using Tomato.Rpc.ServiceDiscovery;

namespace Tomato.Extra
{
    public class ConsulServiceRegistrationProvider:IServiceRegistrationProvider
    {
        private readonly IConsulClient _consul;
        private readonly Action<IRouterPoint, List<AgentServiceCheck>> _serviceCheckAction;


        public ConsulServiceRegistrationProvider(IConsulClient consul,
            Action<IRouterPoint,List<AgentServiceCheck>> serviceCheckAction =null)
        {
            this._consul = consul;
            this._serviceCheckAction = serviceCheckAction;
        }


        public Task RegisterServiceAsync(string serviceId,string serviceName, IRouterPoint point)
        {
            /*
            var httpCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                Interval = TimeSpan.FromSeconds(30),
                HTTP = new Uri(address, "HealthCheck").OriginalString
            };
            */

            string address = EndPointParser.ParseEndPointToIPString(point.RemoteAddress);

            var list = new List<AgentServiceCheck>();
            this._serviceCheckAction?.Invoke(point,list);

            var registration = new AgentServiceRegistration
            {
                Checks = list.ToArray(),
                Address = address,
                ID = serviceId,
                Name = serviceName,
                Port = point.RemoteAddress.Port
            };

            return this._consul.Agent.ServiceRegister(registration);
        }

        public Task DeregisterServiceAsync(string serviceId)
        {
            return this._consul.Agent.ServiceDeregister(serviceId);
        }

    }
}
