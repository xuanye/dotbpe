namespace DotBPE.Rpc
{
    /// <summary>
    /// 消息序列化器
    /// </summary>
    public interface IMessageParser<TMessage> where TMessage : InvokeMessage
    {
        /// <summary>
        /// 消息转换成Json字符串
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        string ToJson(TMessage msg);

        /// <summary>
        /// HTTP请求转换成TMessage格式
        /// </summary>
        /// <param name="rd"></param>
        /// <returns></returns>
        TMessage ToMessage(RequestData rd);
    }
}
