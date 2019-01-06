namespace DotBPE.Gateway
{
    public class Constants
    {


        public const string REQUEST_ID_HEAD_NAME = "X-Request-Id";

        /// <summary>
        /// 请求ID字段
        /// </summary>
        public const string REQUEST_ID_FIELD_NAME = "x_request_id";

        /// <summary>
        /// 用户标识字段
        /// </summary>
        public const string IDENTITY_FIELD_NAME = "identity";

        /// <summary>
        /// CLIENT_IP字段
        /// </summary>
        public const string CLIENT_IP_FIELD_NAME = "client_ip";

        /// <summary>
        /// 消息中可以提取的返回码字段
        /// </summary>
        public const string CODE_FIELD_NAME = "return_code";

        /// <summary>
        /// 消息中可以提取的返回说明字段
        /// </summary>
        public const string MESSAGE_FIELD_NAME = "return_msg";
    }

}
