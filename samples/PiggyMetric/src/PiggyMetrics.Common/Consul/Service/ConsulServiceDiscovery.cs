using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;

namespace PiggyMetrics.Common.Consul.Service
{
    public class ConsulServiceDiscovery: IServiceDiscovery
    {
        private readonly ConsulClient _client;

        private readonly string _serviceCategory ;
        private ulong _lastIndex;
        private readonly HashSet<string> _requireServices;
        private QueryOptions _queryOptions;
        public ConsulServiceDiscovery(string serviceCategory,string requireServices,Action<ConsulClientConfiguration> configOverride)
        {
            this._serviceCategory = serviceCategory;
            this._client = new ConsulClient(configOverride);

             _requireServices = new HashSet<string>();
            _queryOptions = new QueryOptions(){
                WaitIndex = 0,
                WaitTime = TimeSpan.FromMinutes(5)
            };
            InitRequireServices(requireServices);
        }

        private void InitRequireServices(string requireServices)
        {

            string[] services =requireServices.Split(',');

            foreach(string serviceId in services){
                _requireServices.Add(serviceId);
            }

        }

        public async Task<List<ServiceMeta>> FindAll()
        {
            List<ServiceMeta> list = new List<ServiceMeta>();
            if(_lastIndex>0){
                _queryOptions.WaitIndex = _lastIndex +1;
            }
            var reslut = await this._client.Health.Service(Constants.SERVICE_NAME,_serviceCategory,true,_queryOptions);

            if (reslut.StatusCode == System.Net.HttpStatusCode.OK )
            {
                if (reslut.LastIndex > this._lastIndex)
                {
                    _lastIndex = reslut.LastIndex;
                    if (reslut.Response != null && reslut.Response.Length > 0)
                    {
                        foreach (ServiceEntry entry in reslut.Response)
                        {
                            string[] splitId =  entry.Service.ID.Split('$');
                            if(splitId.Length !=2){
                                continue;
                            }
                            string serviceId = splitId[1];
                            if(!this._requireServices.Contains(serviceId)){
                                continue;
                            }
                            ServiceMeta meta = new ServiceMeta
                            {
                                Id = entry.Service.ID,
                                ServiceName =  splitId[0],
                                Address= entry.Service.Address,
                                Port = entry.Service.Port
                            };
                            list.Add(meta);
                        }
                    }
                }
            }
            else if
                (reslut.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                //找不到的话 就不做处理了
            }
            else
            {
                throw new Exception($"call find services error,return code {reslut.StatusCode}");
            }
            return list;
        }
    }
}
