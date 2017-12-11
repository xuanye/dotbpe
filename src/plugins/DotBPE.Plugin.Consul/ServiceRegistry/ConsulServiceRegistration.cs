using System;
using System.Threading.Tasks;
using Consul;
using DotBPE.Rpc.ServiceRegistry;

namespace DotBPE.Plugin.Consul.ServiceRegistry
{
    public class ConsulServiceRegistration: IServiceRegistrationProvider
    {
        private readonly ConsulClient _client;

        private readonly string _serviceCategory;

        public ConsulServiceRegistration(string serviceCategory, Action<ConsulClientConfiguration> configOverride)
        {
            this._serviceCategory = serviceCategory;
            this._client = new ConsulClient(configOverride);
        }      

        public async Task RegisterAsync(ServiceMeta service)
        {
            await this._client.Agent.ServiceDeregister(service.ServiceId.ToString());


            var reg = new AgentServiceRegistration
            {
                ID = service.Id,
                Name = service.ServiceName,
                Address = service.IPAddress,
                Port = service.Port,
                Tags = service.Tags
            };
            reg.Check = new AgentServiceCheck();
            reg.Check.DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(50);
            reg.Check.TCP = $"{service.IPAddress}:{service.Port}";
            reg.Check.Interval = TimeSpan.FromSeconds(10);

            await this._client.Agent.ServiceRegister(reg);
        }
    }
}
