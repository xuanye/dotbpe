using System.Collections.Generic;

namespace DotBPE.Gateway
{
    public interface IHttpRequestData
    {
        int ServiceId { get; }

        ushort MessageId { get; }

        IDictionary<string,string> QueryOrFormData { get; }

        string RawBody { get; }
    }
}
