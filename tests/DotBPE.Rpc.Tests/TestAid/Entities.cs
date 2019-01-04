using System.Runtime.Serialization;

namespace DotBPE.Rpc.Tests
{

    [DataContract]
    public class FooReq
    {
        [DataMember(Order = 1,Name = "foo_word")]
        public string FooWord { get; set; }
    }
    [DataContract]
    public class FooRes
    {
        [DataMember(Order = 1,Name = "ret_foo_word")]
        public string RetFooWord { get; set; }
    }

    public class FooOptions
    {
        public string Option1 { get; set; }
        public int Option2 { get; set; }
    }

}
