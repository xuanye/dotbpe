using System;

namespace DotBPE.Rpc.Server
{
    public interface IRequestAuditLogger: IDisposable
    {
        string MethodFullName { get; }

        void SetParameter(object parameter);
        void SetReturnValue(object retVal);
    }
}
