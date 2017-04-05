using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Codes.Avenue
{
    public static class Constants
    {
        /// <summary>
        /// 标准头长度
        /// </summary>
        internal static readonly int STANDARD_HEADLEN = 44;


        /// <summary>
        /// 最大包长
        /// </summary>
        internal static readonly int MAX_FRAME_SIZE = 2000000;

        /// <summary>
        /// 标准头中的类型，标识为请求
        /// </summary>
        internal static readonly int TYPE_REQUEST = 0xA1;
        /// <summary>
        /// 标准头中的类型，标识为响应
        /// </summary>
        internal static readonly int TYPE_RESPONSE = 0xA2;

        /// <summary>
        /// 路由标识
        /// </summary>
        internal static readonly int ROUTE_FLAG = 0x0F;

        /// <summary>
        /// 版本 固定为1
        /// </summary>
        internal static readonly int VERSION_1 = 1;

        /// <summary>
        /// Body体格式为TLV格式
        /// </summary>
        internal static readonly int FORMAT_TLV = 0;
        /// <summary>
        /// Body体格式为JSON
        /// </summary>
        internal static readonly int FORMAT_JSON = 1;

        /// <summary>
        /// 消息体编码GBK
        /// </summary>
        internal static readonly int ENCODING_GBK = 0;
        /// <summary>
        /// 消息体编码UTF8
        /// </summary>
        internal static readonly int ENCODING_UTF8 = 1;

        /// <summary>
        /// 是否必达标识 否
        /// </summary>
        internal static readonly int MUSTREACH_NO = 0;
        /// <summary>
        /// 是否必达标识 是
        /// </summary>
        internal static readonly int MUSTREACH_YES = 1;

        internal static readonly byte[] EMPTY_SIGNATURE = new byte[16];
    }
}
