using System;

namespace Tomato.Rpc.Client
{
    public interface IClientAuditLogger:IDisposable
    {
        string MethodFullName { get; }

        void SetParameter(object parameter);
        void SetReturnValue(object retVal);
    }

}
