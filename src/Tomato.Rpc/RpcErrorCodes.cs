using System;
using System.Collections.Generic;
using System.Text;

namespace Tomato.Rpc
{
    public class RpcErrorCodes
    {
        /// <summary>
        /// internal error 
        /// </summary>
        public const int CODE_INTERNAL_ERROR = -10242500; 

        /// <summary>
        /// 拒绝访问
        /// </summary>
        public const int CODE_ACCESS_DENIED = -10242403;

        /// <summary>
        /// service not found
        /// </summary>
        public const int CODE_SERVICE_NOT_FOUND = -10242404; 

        /// <summary>
        /// ratelimed
        /// </summary>
        public const int CODE_RATELIMITED = -10242409; 

        /// <summary>
        /// timeout
        /// </summary>
        public const int CODE_TIMEOUT = -10242504; 
    }
}
