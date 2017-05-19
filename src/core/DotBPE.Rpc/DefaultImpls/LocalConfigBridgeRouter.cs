using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Options;
using DotBPE.Rpc.Utils;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net;

namespace DotBPE.Rpc.DefaultImpls
{
    public class LocalConfigBridgeRouter<TMessage> :AbstractBridgeRouter<TMessage> where TMessage:InvokeMessage
    {
        private IOptions<RemoteServicesOption> _options;
        private readonly static Dictionary<string, List<EndPoint>> routerDict = new Dictionary<string, List<EndPoint>>();

        public LocalConfigBridgeRouter(IOptions<RemoteServicesOption> options)
        {
            this._options = options;
            InitRouter();
        }
        private void InitRouter()
        {
            if (this._options != null && this._options.Value != null)
            {
                var routeOptions = this._options.Value;
                foreach (var option in routeOptions)
                {
                    List<EndPoint> address = ParseUtils.ParseEndPointListFromString(option.RemoteAddress);

                    string key;
                    if (string.IsNullOrEmpty(option.MessageIds))
                    {
                        key = option.ServiceId + "$0";
                        AddRouter(key, address);
                    }
                    else
                    {
                        string[] arrMsgIds = option.MessageIds.Split(',');
                        for (int i = 0; i < arrMsgIds.Length; i++)
                        {
                            key = option.ServiceId + "$" + arrMsgIds[i];
                            AddRouter(key, address);
                        }
                    }
                }
            }
        }
        private void AddRouter(string key, List<EndPoint> remoteAddress)
        {
            if (routerDict.ContainsKey(key))
            {
                routerDict[key].AddRange(remoteAddress);
            }
            else
            {
                routerDict.Add(key, remoteAddress);
            }
        }

        public override RouterPoint GetRouterPoint(TMessage message)
        {
            RouterPoint point = new RouterPoint();
            point.RoutePointType = RoutePointType.Local;

            string keyService = message.ServiceIdentifier;
            string keyMessage = message.MethodIdentifier;

            if (routerDict.ContainsKey(keyMessage))
            {
                point.RoutePointType = RoutePointType.Remote;
                point.RemoteAddress = ChooseEndPoint(keyService, routerDict[keyMessage]);
                return point;
            }

            if (routerDict.ContainsKey(keyService))
            {
                point.RoutePointType = RoutePointType.Remote;
                point.RemoteAddress = ChooseEndPoint(keyService, routerDict[keyService]);
                return point;
            }

            return point;
        }
   
    }
}
