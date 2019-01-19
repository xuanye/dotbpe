using System.Runtime.Serialization;

namespace DotBPE.Rpc.BestPractice
{
    [DataContract]
    public class JsonResult:IJsonResult
    {
        [DataMember(Order = 1,Name = "code")]
        public int Code { get; set; }
        [DataMember(Order = 2,Name = "message")]
        public string Message { get; set; }

        [DataMember(Order = 3,Name = "data")]
        public  object Data { get; set; }
    }

    public interface IJsonResult
    {
        int Code { get; set; }

        string Message { get; set; }

        object Data { get; set; }
    }

}
