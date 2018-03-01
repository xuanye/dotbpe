using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using System;
using System.Collections.Generic;
using System.Text;

namespace MathCommon
{
    public class AuditLoggerFormat : IAuditLoggerFormat<AmpMessage>
    {
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
            var jsonReq = AmpMessageHelper.Stringify(req);
            var jsonRsp = AmpMessageHelper.Stringify(rsp);

            return string.Format("{0}  ,{1}  ,{2}  ,req={3}  ,res={4}  ,{5}",req.Id, mothedName, jsonReq, jsonRsp, elapsedMS, rsp.Code);
        }

        private string FormatRequestLog(IRpcContext context,  AmpMessage req, AmpMessage rsp, long elapsedMS)
        {
            string ip = "unknown";
            if(context !=null && context.RemoteAddress != null)
            {
                ip = DotBPE.Rpc.Utils.ParseUtils.ParseEndPointToIPString(context.RemoteAddress);
            }
            string mothedName = req.FriendlyServiceName ?? req.MethodIdentifier;
            var jsonReq = AmpMessageHelper.Stringify(req);
            var jsonRsp = AmpMessageHelper.Stringify(rsp);
            //clientIp,reqId,serviceName,request_data,response_data , elapsedMS ,status_code
            return string.Format("{0}  ,{1}  ,{2}  ,req={3}  ,res={4}  ,{5}  ,{6}",ip,req.Id, mothedName, jsonReq, jsonRsp, elapsedMS, rsp.Code);
        }
    }
}
