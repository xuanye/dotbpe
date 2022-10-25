// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System.Reflection;

namespace DotBPE.Rpc.AuditLog
{

    public class AuditLogInfo : IAuditLogInfo
    {
        public string MethodName { get; set; }

        public IRpcContext Context { get; set; }

        public object Request { get; set; }
        public object Response { get; set; }
        public int StatusCode { get; set; }
        public long ElapsedMS { get; set; }
        public AuditLogType AuditLogType { get; set; }
    }

    public interface IAuditLogInfo
    {
        string MethodName { get; }
        IRpcContext Context { get; }
        object Request { get; }
        object Response { get; }
        int StatusCode { get; }
        long ElapsedMS { get; }
        AuditLogType AuditLogType { get; }
    }

    public enum AuditLogType
    {
        Client,
        Service,
        InProc
    }
}
