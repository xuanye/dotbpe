using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Gateway
{
    public interface IHttpApiErrorProcess
    {
        Task<bool> ProcessAsync(HttpResponse response, Encoding encoding, Error e);
    }

    [DataContract]
    public class Error
    {
        [DataMember(Order = 1, Name = "code")]
        public StatusCode ErrorCode { get; set; }

        [DataMember(Order = 1, Name = "message")]
        public string Message { get; set; }
    }

}
