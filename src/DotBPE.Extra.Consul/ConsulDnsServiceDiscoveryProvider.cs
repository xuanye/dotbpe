using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DnsClient;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.ServiceDiscovery;

namespace DotBPE.Extra
{
    public class ConsulDnsServiceDiscoveryProvider:IServiceDiscoveryProvider
    {
        private readonly IDnsQuery _dnsQuery;
        private const string baseDomain = "service.consul";

        public ConsulDnsServiceDiscoveryProvider(IDnsQuery dnsQuery)
        {
            this._dnsQuery = dnsQuery;
        }
        public async Task<IRouterPoint> LookupServiceDefaultEndPointAsync(string serviceName)
        {
            var listRsp = await this._dnsQuery.ResolveServiceAsync(baseDomain, serviceName);

            if (!(listRsp != null & listRsp.Any())) return null;

            //DNS本身已经处理了负载均衡
            //所以始终返回第一个
            var point = new RouterPoint
            {
                RoutePointType = RoutePointType.Remote,
                RemoteAddress = new IPEndPoint(listRsp[0].AddressList[0], listRsp[0].Port)
            };


            return point;

        }

        public async Task<List<IRouterPoint>> LookupServiceEndPointListAsync(string serviceName)
        {
            var listRsp = await this._dnsQuery.ResolveServiceAsync(baseDomain, serviceName);

            if (!(listRsp != null & listRsp.Any())) return null;

            var retLst = new List<IRouterPoint>();

            foreach (var item in listRsp)
            {
                if (item.AddressList != null && item.AddressList.Any())
                {
                    retLst.AddRange(item.AddressList.Select(address =>
                        new RouterPoint
                        {
                            RoutePointType = RoutePointType.Remote, RemoteAddress = new IPEndPoint(address, item.Port)
                        }));
                }

            }
            return retLst;
        }
    }
}
