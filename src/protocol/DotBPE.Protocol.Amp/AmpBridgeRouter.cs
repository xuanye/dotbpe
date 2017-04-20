using DotBPE.Rpc;
using DotBPE.Rpc.Options;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net;
using DotBPE.Rpc.Utils;

namespace DotBPE.Protocol.Amp
{
    /// <summary>
    /// 通过ServiceId和MessageId在Config中查找配置
    /// </summary>
    public class AmpBridgeRouter : IBridgeRouter<AmpMessage>
    {
        private IOptions<RemoteServicesOption> _options;
        private readonly static Dictionary<string, List<EndPoint>> routerDict = new Dictionary<string, List<EndPoint>>();
        private readonly static Dictionary<string, int> chooseRandom = new Dictionary<string, int>();
        private readonly object lockObject = new object();
        public AmpBridgeRouter(IOptions<RemoteServicesOption> options){
            this._options = options;
            InitRouter();
        }
        private void InitRouter(){
            if(this._options !=null && this._options.Value !=null)
            {
                var routeOptions = this._options.Value;
                foreach ( var option in routeOptions)
                {
                    List<EndPoint> address =ParseUtils.ParseEndPointListFromString(option.RemoteAddress);

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
                            key = option.ServiceId + "$"+ arrMsgIds[i];
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

        public RouterPoint GetRouterPoint(AmpMessage message)
        {
            RouterPoint point  = new RouterPoint();
            point.RoutePointType = RoutePointType.Local;

            string keyService = $"{message.ServiceId}$0";
            string msgKey = $"{message.ServiceId}${message.MessageId}";
            if (routerDict.ContainsKey(msgKey))
            {
                point.RoutePointType = RoutePointType.Remote;
                point.RemoteAddress = ChooseEndPoint(keyService, routerDict[msgKey]);
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

        /// <summary>
        /// 项目中可能需要更加复杂的计算算法,可以考虑服务的有效性，或者可以设置权重什么的
        /// </summary>
        /// <param name="key"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        protected virtual EndPoint ChooseEndPoint(string key,List<EndPoint> list)
        {
            int chooseIndex = 0;
            lock (lockObject)
            {
                if (!chooseRandom.ContainsKey(key))
                {
                    chooseRandom.Add(key, chooseIndex);
                }
                else
                {
                    chooseIndex = chooseRandom[key]++;
                    if (chooseIndex >= list.Count)
                    {
                        chooseIndex = 0;
                    }
                    chooseRandom[key] = chooseIndex;
                }
            }
            return list[chooseIndex];

        }
    }
}