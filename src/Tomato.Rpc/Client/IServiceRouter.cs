using Peach.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tomato.Rpc.Client
{
    /// <summary>
    /// 服务路由接口
    /// </summary>
    public interface IServiceRouter
    {
        Task<IRouterPoint> FindRouterPoint(string servicePath);
    }
}
