using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace DotBPE.Gateway
{
    public class DefaultHttpRequestData:IHttpRequestData
    {
        public IDictionary<string, string> QueryOrFormData { get; }
        public string JSONBody { get; }

        public static DefaultHttpRequestData Parse(HttpRequest request)
        {
            return new DefaultHttpRequestData();
        }
    }
}
