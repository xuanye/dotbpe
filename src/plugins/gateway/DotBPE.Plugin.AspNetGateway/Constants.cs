namespace DotBPE.Plugin.AspNetGateway
{
    public class Constants
    {
        public const string DOTBPE_SEESIONID = "bpe-session-id";

        public const string SEESIONID_FIELD_NAME = "bpe_session_id";

        public const string X_REQUEST_ID = "x-request-id";

        /// <summary>
        /// 请求ID字段
        /// </summary>
        public const string REQUESTID_FIELD_NAME = "x_request_id";

        /// <summary>
        /// 用户标识字段
        /// </summary>
        public const string IDENTITY_FIELD_NAME = "identity";

        /// <summary>
        /// CLIENT_IP字段
        /// </summary>
        public const string CLIENTIP_FIELD_NAME = "client_ip";

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
