using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PiggyMetrics.Common.Consul.Service
{
    public interface IServiceDiscovery
    {
        Task<List<ServiceMeta>> FindAll();
    }
}
