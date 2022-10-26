// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System.Runtime.Serialization;

namespace MathService.Definition
{
    /// <summary>
    /// 加法返回值
    /// </summary>
    [DataContract]
    public class SumRes
    {
        /// <summary>
        /// 总记录
        /// </summary>
        [DataMember(Order = 1, Name = "total")]
        public int Total { get; set; }

    }

    /// <summary>
    /// 加法请求
    /// </summary>
    [DataContract]
    public class SumReq
    {
        /// <summary>
        /// 字段A
        /// </summary>
        [DataMember(Order = 1, Name = "a")]
        public int A { get; set; }

        /// <summary>
        /// 字段B
        /// </summary>
        [DataMember(Order = 2, Name = "b")]
        public int B { get; set; }
    }


}
