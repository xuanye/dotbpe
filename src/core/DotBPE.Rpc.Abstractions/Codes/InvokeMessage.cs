/// <summary>
/// 
/// </summary>
namespace DotBPE.Rpc.Codes
{

    /// <summary>
    /// 传输消息的抽象类，必须包含这些内容
    /// </summary>
    public abstract class InvokeMessage
    {
        public InvokeMessageType InvokeMessageType { get; set; }

        public abstract int Length { get; }

        /// <summary>
        /// 服务识别符,定位到某个服务
        /// </summary>
        public abstract string ServiceIdentifier { get; }

        /// <summary>
        /// 方法识别符，定位到某个方法
        /// </summary>
        public abstract string MethodIdentifier { get; }

        public abstract bool IsHeartBeat { get; }
    }

  

    public enum InvokeMessageType : byte
    {
        Request = 1,
        Response = 2,
        Notify = 3
    }
}
