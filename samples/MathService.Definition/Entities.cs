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
        [DataMember(Order =1,Name ="total")]
        public int Total { get; set; }

        /// <summary>
        /// FooWord字段
        /// </summary>
        [DataMember(Order = 2, Name = "foo_word")]
        public string FooWord { get; set; }
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

    /// <summary>
    /// Foo请求
    /// </summary>
    [DataContract]
    public class FooReq
    {
        /// <summary>
        /// FooWord字段
        /// </summary>
        [DataMember(Order = 1, Name = "foo_word")]
        public string FooWord{ get; set; }
    }

    /// <summary>
    /// Foo返回值
    /// </summary>
    [DataContract]
    public class FooRes
    {
        /// <summary>
        /// 返回值字段
        /// </summary>
        [DataMember(Order = 1, Name = "ret_word")]
        public string RetWord { get; set; }
    }
}
