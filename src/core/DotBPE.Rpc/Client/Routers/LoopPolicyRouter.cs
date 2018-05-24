using DotBPE.Rpc.Options;
using DotBPE.Rpc.Utils;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net;

namespace DotBPE.Rpc.Client
{
    public class LoopPolicyRouter<TMessage> : IRouter<TMessage> where TMessage : InvokeMessage
    {
        private IOptions<RemoteServicesOption> _options;
        private readonly static Dictionary<string, List<EndPoint>> routerDict = new Dictionary<string, List<EndPoint>>();
        private readonly static Dictionary<string, int> chooseRandom = new Dictionary<string, int>();
        private readonly object lockObject = new object();

        public LoopPolicyRouter(IOptions<RemoteServicesOption> options)
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
                    if (string.IsNullOrEmpty(option.MessageId))
                    {
                        key = option.ServiceId + "$0";
                        AddRouter(key, address);
                    }
                    else
                    {
                        string[] arrMsgIds = option.MessageId.Split(',');
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

        public RouterPoint GetRouterPoint(TMessage message)
        {
            RouterPoint point = new RouterPoint();
            point.RoutePointType = RoutePointType.Local;

            string keyService = message.ServiceIdentifier;
            string keyMessage = message.MethodIdentifier;

            if (routerDict.ContainsKey(keyMessage))
            {
                point.RoutePointType = RoutePointType.Remote;
                point.RemoteAddress = ChooseEndPoint(keyMessage, routerDict[keyMessage]);
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
        /// 轮询选择一个远端节点
        /// 项目中可能需要更加复杂的计算算法,可以考虑服务的负载情况，或者可以设置权重什么的
        /// </summary>
        /// <param name="key">服务标识 serviceId$0</param>
        /// <param name="list">该服务标识所有的服务地址</param>
        /// <returns>选中的服务地址</returns>
        protected virtual EndPoint ChooseEndPoint(string key, List<EndPoint> list)
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
                    chooseIndex = chooseRandom[key] + 1;
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
