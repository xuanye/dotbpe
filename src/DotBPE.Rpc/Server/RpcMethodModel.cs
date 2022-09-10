using System.Collections.Generic;

namespace DotBPE.Rpc.Server
{
    internal class RpcMethodModel
    {
        public RpcMethodModel(IMethod method, string pattern, IList<object> metadata, RpcRequestDelegate requestDelegate)
        {
            Method = method;
            Pattern = pattern;
            Metadata = metadata;
            RequestDelegate = requestDelegate;
        }

        public IMethod Method { get; }
        public string Pattern { get; }
        public IList<object> Metadata { get; }
        public RpcRequestDelegate RequestDelegate { get; }
    }
}
