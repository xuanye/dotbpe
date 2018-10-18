using System;
using System.Net;
using System.Threading.Tasks;

namespace DotBPE.Rpc
{
    public interface IClientBootstrap<TMessage> : IClientBootstrap where TMessage : InvokeMessage
    {
        /// <summary>
        /// 发起链接
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <returns></returns>
        IRpcContext<TMessage> GetContext(EndPoint remotePoint);

        /// <summary>
        /// 断开链接时的事件
        /// </summary>
        event EventHandler<ConnectionEventArgs> DisConnected;
    }

    public interface IClientBootstrap : IDisposable
    {
        Task StartAsync();
        Task StopAsync();
    }
}
