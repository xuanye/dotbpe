using Peach.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    /// <summary>
    /// 服务路由接口
    /// </summary>
    public interface IServiceRouter<TMessage> where TMessage:IMessage
    {
        Task<IRouterPoint> FindRouterPoint(TMessage message);
    }
}
