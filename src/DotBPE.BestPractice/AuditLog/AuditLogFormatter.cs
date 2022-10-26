// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using DotBPE.Rpc.AuditLog;
using DotBPE.Rpc.Server;
using Google.Protobuf;

namespace DotBPE.BestPractice.AuditLog
{
    public class AuditLogFormatter : IAuditLogFormatter
    {
        private static readonly AuditJsonFormatter _jsonFormatter = new AuditJsonFormatter(new AuditJsonFormatter.Settings(false).WithFormatEnumsAsIntegers(true));
        public string Format(IAuditLogInfo auditLog)
        {
            string remoteIP = "Local";

            if (auditLog.Context != null && auditLog.Context.GetType() != typeof(LocalRpcContext))
            {
                remoteIP = auditLog.Context.RemoteAddress.Address.MapToIPv4().ToString();
            }

            var reqMsg = auditLog.Request as IMessage;

            var jsonReq = reqMsg == null ? "{}" : _jsonFormatter.Format(reqMsg);
            var jsonRsp = !(auditLog.Response is IMessage resMsg) ? "{}" : _jsonFormatter.Format(resMsg);

            var clientIP = FindFieldValue(reqMsg, "client_ip");
            var requestId = FindFieldValue(reqMsg, "x_request_id");
            if (string.IsNullOrEmpty(clientIP))
            {
                clientIP = "UNKNOWN";
            }
            if (string.IsNullOrEmpty(requestId))
            {
                requestId = "UNKNOWN";
            }
            //remoteIP,clientIp,requestId,serviceName,request_data,response_data , elapsedMS ,status_code
            return string.Format("{0},  {1},  {2},  {3},  req={4},  res={5},  {6},  {7}",
                remoteIP, clientIP, requestId, auditLog.MethodName, jsonReq, jsonRsp, auditLog.ElapsedMS, auditLog.StatusCode);
        }

        private static string FindFieldValue(IMessage msg, string fieldName)
        {
            if (msg == null)
            {
                return "";
            }
            var field = msg.Descriptor.FindFieldByName(fieldName);
            if (field != null)
            {
                var retObjV = field.Accessor.GetValue(msg);
                if (retObjV != null)
                {
                    return retObjV.ToString();
                }
            }
            return "";
        }
    }
}
