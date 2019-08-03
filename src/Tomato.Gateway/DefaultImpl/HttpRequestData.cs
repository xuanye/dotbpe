using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Tomato.Gateway
{
    public class HttpRequestData:IHttpRequestData
    {
        public int ServiceId { get; set; }
        public ushort MessageId { get; set; }
        public IDictionary<string, string> QueryOrFormData { get;  } = new Dictionary<string, string>();
        public string RawBody { get; set; }

    }
}
