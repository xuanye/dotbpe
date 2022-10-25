// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using DotBPE.Rpc.AuditLog;
using DotBPE.Rpc.Server;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.BestPractice.AuditLog
{
    public class AuditLogFormatter : IAuditLogFormatter
    {
        private static readonly AuditJsonFormatter _jsonFormatter = new AuditJsonFormatter(new AuditJsonFormatter.Settings(false).WithFormatEnumsAsIntegers(true));
        public string Format(IAuditLogInfo auditLog)
        {
            throw new NotImplementedException();

        }
        private string FormatCACLog(string methodName, object req, RpcResult<object> rsp, long elapsedMS)
        {
            IMessage reqMsg = req as IMessage;
            IMessage resMsg = null;
            if (rsp.Data != null)
            {
                resMsg = rsp.Data as IMessage;
            }

            var jsonReq = reqMsg == null ? "" : JsonFormatter.Format(reqMsg);
            var jsonRsp = resMsg == null ? "" : JsonFormatter.Format(resMsg);

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
            //clientIp,requestId,serviceName, elapsedMS,status_code
            return string.Format("{0},  {1},  {2},  req={3},  res={4},  {5},  {6}", clientIP, requestId, methodName, jsonReq, jsonRsp, elapsedMS, rsp.Code);
        }

        private string FormatRequestLog(IRpcContext context, string methodName, object req, RpcResult<object> rsp, long elapsedMS)
        {
            string remoteIP = "Local"; //本地服务间调用        

            if (context != null && context.GetType() != typeof(LocalRpcContext))
            {
                remoteIP = context.RemoteAddress.Address.MapToIPv4().ToString();
            }

            IMessage reqMsg = req as IMessage;
            IMessage resMsg = null;
            if (rsp.Data != null)
            {
                resMsg = rsp.Data as IMessage;
            }

            var jsonReq = reqMsg == null ? "" : JsonFormatter.Format(reqMsg);
            var jsonRsp = resMsg == null ? "" : JsonFormatter.Format(resMsg);

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
            return string.Format("{0},  {1},  {2},  {3},  req={4},  res={5},  {6},  {7}", remoteIP, clientIP, requestId, methodName, jsonReq, jsonRsp, elapsedMS, rsp.Code);
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
