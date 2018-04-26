using DotBPE.Protobuf;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using Google.Protobuf;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace MathCommon
{
    public class AuditLoggerFormat : IAuditLoggerFormat<AmpMessage>
    {
        private static readonly JsonFormatter JsonFormatter = new JsonFormatter(new JsonFormatter.Settings(false).WithFormatEnumsAsIntegers(true));

        private readonly IProtobufObjectFactory _factory;

        private static ConcurrentDictionary<string, Google.Protobuf.MessageParser> tempCache = new ConcurrentDictionary<string, Google.Protobuf.MessageParser>();


        public AuditLoggerFormat(IProtobufObjectFactory factory)
        {
            _factory = factory;
        }

        public string Format(IRpcContext context, AuditLogType logType, AmpMessage req, AmpMessage rsp, long elapsedMS)
        {
            string log = string.Empty;

            if (req == null || rsp == null)
            {
                return log;
            }
            var reqParser = GetMessageParser(req.ServiceId, req.MessageId, 1);
            var resParser = GetMessageParser(rsp.ServiceId, rsp.MessageId, 2);

            if (reqParser == null || resParser == null)
            {
                Console.WriteLine("{0},{1},没有生成对应的ProtoObjectFactory方法，请检查配置", req.ServiceId, req.MessageId);
                return null;
            }

            if (logType == AuditLogType.CACAduit)
            {
                log = FormatCACLog(req, rsp, elapsedMS, reqParser, resParser);
            }
            else if (logType == AuditLogType.RequestAudit)
            {
                log = FormatRequestLog(context, req, rsp, elapsedMS, reqParser, resParser);
            }

            return log;
        }

        private string FormatCACLog(AmpMessage req, AmpMessage rsp, long elapsedMS, Google.Protobuf.MessageParser reqParser, Google.Protobuf.MessageParser resParser)
        {
            string mothedName = req.FriendlyServiceName ?? req.MethodIdentifier;

            IMessage reqMsg = null;
            IMessage resMsg = null;
            if (req.Data != null)
            {
                reqMsg = reqParser.ParseFrom(req.Data);
            }
            if (rsp.Data != null)
            {
                resMsg = resParser.ParseFrom(rsp.Data);
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
            return string.Format("{0},  {1},  {2},  req={3},  res={4},  {5}", clientIP, requestId, mothedName, jsonReq, jsonRsp, elapsedMS, rsp.Code);
        }


        private string FormatRequestLog(IRpcContext context, AmpMessage req, AmpMessage rsp, long elapsedMS, Google.Protobuf.MessageParser reqParser, Google.Protobuf.MessageParser resParser)
        {
            string remoteIP = "UNKNOWN";

            if (context != null && context.RemoteAddress != null)
            {
                remoteIP = DotBPE.Rpc.Utils.ParseUtils.ParseEndPointToIPString(context.RemoteAddress);
            }
            string mothedName = req.FriendlyServiceName ?? req.MethodIdentifier;
            IMessage reqMsg = null;
            IMessage resMsg = null;
            if (req.Data != null)
            {
                reqMsg = reqParser.ParseFrom(req.Data);
            }
            if (rsp.Data != null)
            {
                resMsg = resParser.ParseFrom(rsp.Data);
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
            return string.Format("{0},  {1},  {2},  {3},  req={4},  res={5},  {6},  {7}", remoteIP, clientIP, requestId, mothedName, jsonReq, jsonRsp, elapsedMS, rsp.Code);
        }


        private Google.Protobuf.MessageParser GetMessageParser(ushort serviceId, ushort messageId, int type)
        {
            string cacheKey = string.Format("{0}_{1}_{2}", serviceId, messageId, type);
            Google.Protobuf.MessageParser tmp = null;
            if (tempCache.ContainsKey(cacheKey))
            {
                tempCache.TryGetValue(cacheKey, out tmp);
            }

            if (tmp == null)
            {
                IMessage tMsg = null;
                if (type == 1)
                {
                    tMsg = _factory.GetRequestTemplate(serviceId, messageId);
                }
                else
                {
                    tMsg = _factory.GetResponseTemplate(serviceId, messageId);
                }
                if (tMsg != null)
                {
                    tmp = tMsg.Descriptor.Parser;
                    tempCache.TryAdd(cacheKey, tmp);
                }
            }
            return tmp;
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
