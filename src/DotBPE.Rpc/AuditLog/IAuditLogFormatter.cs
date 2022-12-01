// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.AuditLog
{
    public interface IAuditLogFormatter
    {
        string Format(IAuditLogInfo auditLog);
    }
}
