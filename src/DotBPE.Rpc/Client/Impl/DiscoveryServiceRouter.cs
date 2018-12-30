using DotBPE.Rpc.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    /// <summary>
    /// 基于服务注册和发现的服务路由
    /// </summary>
    public class DiscoveryServiceRouter : IServiceRouter
    {
        public IRouterPoint FindRouterPoint(string servicePath)
        {
            throw new NotImplementedException();
        }
    }
}
