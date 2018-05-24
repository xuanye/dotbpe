using System;
using System.Net;
using System.Threading.Tasks;

namespace DotBPE.Rpc
{
    public interface IClientBootstrap<TMessage> : IDisposable where TMessage : InvokeMessage
    {
        /// <summary>
        /// 发起链接
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <returns></returns>
        Task<IRpcContext<TMessage>> StartConnectAsync(EndPoint remotePoint);

        /// <summary>
        /// 断开链接时的事件
        /// </summary>
        event EventHandler<ConnectionEventArgs> DisConnected;
    }
}
