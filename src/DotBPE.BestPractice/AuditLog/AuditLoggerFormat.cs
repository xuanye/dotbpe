// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using DotBPE.Rpc.Server;
using Google.Protobuf;
using System.Collections.Concurrent;

namespace DotBPE.BestPractice.AuditLog
{
    /// <summary>
    /// 默认的日志格式化类
    /// </summary>   
    public class AuditLoggerFormat : IAuditLoggerFormatter
    {
        private static readonly AuditJsonFormatter JsonFormatter = new AuditJsonFormatter(new AuditJsonFormatter.Settings(false).WithFormatEnumsAsIntegers(true));

        public string Format(IRpcContext context, AuditLogType logType, string methodName, object req, RpcResult<object> rsp, long elapsedMS)
        {
            //System.Console.WriteLine($"-----------------{methodName}--------------------");
            string log = string.Empty;

            if (req == null || rsp == null)
            {
                return log;
            }

            if (logType == AuditLogType.ClientCall)
            {
                log = FormatCACLog(methodName, req, rsp, elapsedMS);
            }
            else if (logType == AuditLogType.ServiceReceive)
            {
                log = FormatRequestLog(context, methodName, req, rsp, elapsedMS);
            }
            // System.Console.WriteLine(log);
            return log;
        }




    }
}
