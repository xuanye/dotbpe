using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{

    public interface ICallInvoker
    {
        /// <summary>
        /// async call and without response
        /// </summary>
        /// <param name="callName">method fullName</param>
        /// <param name="serviceId">rpc service id</param>
        /// <param name="messageId">rpc message id</param>
        /// <param name="req">req obj</param>
        /// <typeparam name="T">message type</typeparam>
        /// <returns>call result</returns>
        Task<RpcResult> AsyncNotify<T>(string callName,string groupName,ushort serviceId,ushort messageId,T req);

        /// <summary>
        /// async call and wait for response
        /// <param name="callName">method fullName</param>
        /// <param name="serviceId">rpc service id</param>
        /// <param name="messageId">rpc message id</param>
        /// <param name="req">req obj</param>
        /// <typeparam name="T">message type</typeparam>
        /// <typeparam name="TResult">result data type</typeparam>
        /// <returns>call result with data</returns>
        Task<RpcResult<TResult>> AsyncRequest<T,TResult>(string callName,string groupName,
            ushort serviceId, ushort messageId,T req, int timeout = 3000) ;
    }
}
