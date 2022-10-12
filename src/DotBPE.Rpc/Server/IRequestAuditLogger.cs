// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Server
{
    public interface IRequestAuditLogger : IDisposable
    {
        string MethodFullName { get; }
        void SetContext(IRpcContext context);
        void SetParameter(object parameter);
        void SetReturnValue(object retVal);
    }
}
