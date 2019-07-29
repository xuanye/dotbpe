using System.Collections.Generic;

namespace Tomato.Gateway
{
    public interface IHttpRequestData
    {
        int ServiceId { get; }

        ushort MessageId { get; }

        IDictionary<string,string> QueryOrFormData { get; }

        string RawBody { get; }
    }
}
