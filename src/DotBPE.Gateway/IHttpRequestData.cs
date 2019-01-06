using System.Collections.Generic;

namespace DotBPE.Gateway
{
    public interface IHttpRequestData
    {
        IDictionary<string,string> QueryOrFormData { get; }

        string JSONBody { get; }
    }
}
