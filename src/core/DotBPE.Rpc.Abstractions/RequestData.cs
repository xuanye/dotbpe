using System.Collections.Generic;

namespace DotBPE.Rpc.Abstractions
{
     public class RequestData
    {
        /// <summary>
        /// 服务ID
        /// </summary>
        /// <returns></returns>
        public int ServiceId { get; set; }
        /// <summary>
        /// 消息ID
        /// </summary>
        /// <returns></returns>
        public int MessageId { get; set; }

        /// <summary>
        /// 请求体中JSON Body
        /// </summary>
        /// <returns></returns>
        public string RawBody { get; set; }

        /// <summary>
        /// 从Request.Query和Request.Form和Request.Head中收集的数据
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> Data { get; set; }
    }
}
