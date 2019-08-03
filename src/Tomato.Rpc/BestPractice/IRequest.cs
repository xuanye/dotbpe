namespace Tomato.Rpc.BestPractice
{
    public interface IRequest
    {
        /// <summary>
        /// 一次请求的唯一标识
        /// </summary>
        string XRequestId { get;  }
        /// <summary>
        /// 最初用户端的IP
        /// </summary>
        string ClientIp { get; }
        /// <summary>
        /// 传递的用户标识
        /// </summary>
        string Identity { get; }
    }
}
