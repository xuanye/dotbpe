// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Server
{
    public interface IRequestAuditLoggerFactory
    {

        IRequestAuditLogger GetLogger(string methodFullName);

    }
}
