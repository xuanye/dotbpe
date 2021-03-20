using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DotBPE.Rpc;

namespace DotBPE.Extra
{
    public interface IPipelineTaskService
    {
        /// <summary>
        /// 插入队列任务
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="messageId"></param>
        /// <param name="executeJson"></param>
        /// <returns></returns>
        Task<RpcResult> Enqueue(int serviceId, ushort messageId, string executeJson);

        /// <summary>
        /// 插入队列任务
        /// </summary>
        /// <param name="delayFunc"></param>
        /// <param name="arg1"></param>
        /// <returns></returns>
        Task<RpcResult> Enqueue<T>(Func<T, Task> delayFunc, T arg1) where T : class;

        Task<RpcResult> EnqueueEx(int serviceId, ushort messageId, string executeJson, string routeKey);

        Task<RpcResult> EnqueueDelayEx(int serviceId, ushort messageId, string executeJson, TimeSpan delayTime, string routeKey);
        /// <summary>
        /// 插入延迟任务
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="messageId"></param>
        /// <param name="executeJson"></param>
        /// <param name="delayTime"></param>
        /// <param name="taskCode"></param>
        /// <param name="checkCode"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        Task<RpcResult> EnqueueDelay(int serviceId, ushort messageId, string executeJson, TimeSpan delayTime,
            string taskCode = null, string checkCode = null, string description = null);

        Task<RpcResult> EnqueueDelay(IDelayTask delayTask);

        /// <summary>
        /// 插入延迟任务
        /// </summary>
        /// <param name="method"></param>
        /// <param name="arg1"></param>
        /// <param name="delayTime"></param>
        /// <param name="taskCode"></param>
        /// <param name="checkCode"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        Task<RpcResult> EnqueueDelay<T>(Func<T, Task> delayFunc, T arg1, TimeSpan delayTime,
            string taskCode = null, string checkCode = null, string description = null) where T : class;


        Task<RpcResult> CancelDelay(string checkCode);
    }
}
