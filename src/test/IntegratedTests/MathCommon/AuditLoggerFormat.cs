using DotBPE.Protobuf;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace MathCommon
{
    public class AuditLoggerFormat : IAuditLoggerFormat<AmpMessage>
    {
        private static readonly JsonFormatter JsonFormatter = new JsonFormatter(new JsonFormatter.Settings(false).WithFormatEnumsAsIntegers(true));

        private readonly IProtobufObjectFactory _factory;
        public AuditLoggerFormat(IProtobufObjectFactory factory)
        {
            _factory = factory;
        }

        public string Format(IRpcContext context, AuditLogType logType, AmpMessage req, AmpMessage rsp, long elapsedMS)
        {
            string log = string.Empty;

            if(logType == AuditLogType.CACAduit)
            {
                log = FormatCACLog(req, rsp, elapsedMS);
            }
            else if(logType == AuditLogType.RequestAudit)
            {
                log = FormatRequestLog(context,req, rsp, elapsedMS);
            }

            return log;
        }

        private string FormatCACLog(AmpMessage req, AmpMessage rsp, long elapsedMS)
        {
            string mothedName = req.FriendlyServiceName ?? req.MethodIdentifier;
            //clientIp,reqId,serviceName, elapsedMS,status_code
            var jsonReq = ToReqJson(req);
            var jsonRsp = ToResJson(rsp);

            return string.Format("{0}  ,{1}  ,{2}  ,req={3}  ,res={4}  ,{5}","TOKEN",req.Id, mothedName, jsonReq, jsonRsp, elapsedMS, rsp.Code);
        }

        private string FormatRequestLog(IRpcContext context,  AmpMessage req, AmpMessage rsp, long elapsedMS)
        {
            string ip = "unknown";
            if(context !=null && context.RemoteAddress != null)
            {
                ip = DotBPE.Rpc.Utils.ParseUtils.ParseEndPointToIPString(context.RemoteAddress);
            }
            string mothedName = req.FriendlyServiceName ?? req.MethodIdentifier;
            var jsonReq = ToReqJson(req);
            var jsonRsp = ToResJson(rsp);
            //clientIp,reqId,serviceName,request_data,response_data , elapsedMS ,status_code
            return string.Format("{0}  ,{1}  ,{2}  ,req={3}  ,res={4}  ,{5}  ,{6}",ip,req.Id, mothedName, jsonReq, jsonRsp, elapsedMS, rsp.Code);
        }

        private string ToResJson(AmpMessage message)
        {
            string ret = string.Empty;
            if (message != null)
            {
                
                var tmp = _factory.GetResponseTemplate(message.ServiceId, message.MessageId);
                if (tmp == null)
                {
                    return ret;
                }

                if (message.Data != null)
                {
                    tmp.MergeFrom(message.Data);
                }

                ret = JsonFormatter.Format(tmp);
            }

            return ret;

        }
        private string ToReqJson(AmpMessage message)
        {
            string ret = string.Empty;
            if (message != null)
            {

                var tmp = _factory.GetRequestTemplate(message.ServiceId, message.MessageId);
                if (tmp == null)
                {
                    return ret;
                }

                if (message.Data != null)
                {
                    tmp.MergeFrom(message.Data);
                }

                ret = JsonFormatter.Format(tmp);
            }

            return ret;
        }
    }
}
