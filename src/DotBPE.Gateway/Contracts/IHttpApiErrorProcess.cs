using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Gateway
{
    public interface IHttpApiErrorProcess
    {
        Task<bool> ProcessAsync(HttpResponse response, Encoding encoding, Error e);
    }
    public class Error
    {
        public StatusCode ErrorCode { get; set; }

        public string Message { get; set; }
    }

}
