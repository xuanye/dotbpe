using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotBPE.Rpc.ServiceRegistry
{
    public interface IServiceDiscoveryProvider
    {
        /// <summary>
        /// 根据服务分类 获取所有相关服务信息
        /// </summary>
        /// <param name="serviceCategory">服务分类</param>
        /// <returns></returns>
        Task<List<ServiceMeta>> FindServicesAsync(string serviceCategory);
    }
}
