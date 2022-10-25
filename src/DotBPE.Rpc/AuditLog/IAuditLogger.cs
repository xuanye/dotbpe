// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc.AuditLog
{
    public interface IAuditLogger
    {
        AuditLogType AuditLogType { get; }
        Task Log(string methodName, object req, object res, int statusCode, long elapsedMS, IRpcContext context);
    }
}
