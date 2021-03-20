using System.Collections.Generic;
using System.Threading.Tasks;
using DotBPE.Rpc.Client;

namespace DotBPE.Rpc.ServiceDiscovery
{
    public interface IServiceDiscoveryProvider
    {
        /// <summary>
        /// lookup service default endpoint by service Name
        /// </summary>
        /// <param name="serviceName">Service Identity</param>
        /// <returns></returns>
        Task<IRouterPoint> LookupServiceDefaultEndPointAsync(string serviceName);

        /// <summary>
        ///  lookup service all endpoint  by service path
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        Task<List<IRouterPoint>> LookupServiceEndPointListAsync(string serviceName);
    }
}
