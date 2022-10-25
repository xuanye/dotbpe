// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc.AuditLog
{
    public interface IAuditLogWriter
    {
        Task WriteAsync(string log);
    }
}
