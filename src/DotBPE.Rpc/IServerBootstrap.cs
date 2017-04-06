using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc
{
    public interface IServerBootstrap:IDisposable
    {
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="endpoint">需要绑定的IP</param>
        /// <returns></returns>
        Task StartAsync(EndPoint endpoint);


    }
}
