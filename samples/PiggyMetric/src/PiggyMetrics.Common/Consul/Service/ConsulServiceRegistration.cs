using System;
using System.Threading.Tasks;
using Consul;

namespace PiggyMetrics.Common.Consul.Service
{
    public class ConsulServiceRegistration: IServiceRegistration
    {
        private readonly ConsulClient _client;

        private readonly string _serviceCategory;

        public ConsulServiceRegistration(string serviceCategory, Action<ConsulClientConfiguration> configOverride)
        {
            this._serviceCategory = serviceCategory;
            this._client = new ConsulClient(configOverride);
        }
        public async Task Register(ServiceMeta service)
        {
            await this._client.Agent.ServiceDeregister(service.ServiceId.ToString());


            var reg = new AgentServiceRegistration
            {
                ID = service.Id ,
                Name = service.ServiceName,
                Address =service.Address,
                Port = service.Port,
                Tags = service.Tags
            };
            reg.Check = new AgentServiceCheck();
            reg.Check.DeregisterCriticalServiceAfter =  TimeSpan.FromSeconds(50) ;
            reg.Check.TCP = $"{service.Address}:{service.Port}";
            reg.Check.Interval = TimeSpan.FromSeconds(10);

            await this._client.Agent.ServiceRegister(reg);
        }
    }
}
