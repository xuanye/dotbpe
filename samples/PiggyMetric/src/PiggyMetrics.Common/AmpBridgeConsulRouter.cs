using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using DotBPE.Rpc.Hosting;
using DotBPE.Rpc.Logging;
using DotBPE.Rpc.Utils;
using Microsoft.Extensions.Options;
using PiggyMetrics.Common.Consul.Service;
using Environment = DotBPE.Rpc.Environment;

namespace PiggyMetrics.Common
{
    public class AmpBridgeConsulRouter:IBridgeRouter<AmpMessage>
    {
        private static readonly ILogger Logger = Environment.Logger.ForType<AmpBridgeConsulRouter>();
        private readonly IServiceDiscovery _discovery;
        private readonly ServiceDiscoveryOption _options;
        private bool _stop= false;
        private Dictionary<string, List<EndPoint>> _routerDict =null;
        private HashSet<string> _remoteList = new HashSet<string>();
        private static readonly Dictionary<string, int> ChooseRandom = new Dictionary<string, int>();
        private static readonly object LockObject = new object();
        private ITransportFactory<AmpMessage> _transportFactory;
        public AmpBridgeConsulRouter(IServiceDiscovery discovery,ITransportFactory<AmpMessage> transportFactory,IOptions<ServiceDiscoveryOption> options)
        {
            _discovery = discovery;
            _options = options.Value ?? ServiceDiscoveryOption.Default;

            _transportFactory = transportFactory;
            Initialize();
        }

        private void Initialize()
        {
            ThreadPool.QueueUserWorkItem(CheckChanged);
        }

        private void CheckChanged(object state)
        {
            while (!_stop)
            {
                LoadServiceMeta().Wait();
                Thread.Sleep(this._options.Interval);
            }
        }

        public async Task LoadServiceMeta()
        {
            var list =  await this._discovery.FindAll();
            if (list.Count == 0)
            {
                return;
            }
            var newRouter = new Dictionary<string, List<EndPoint>>();
            var newList = new List<EndPoint>();
            var allList = new HashSet<string>();
            foreach (var service in list)
            {
                string key = service.ServiceId + "$0";
                Logger.Debug("Load Service Id= {0},Address = {1}", service.ServiceId, service.Host);

                var address = ParseUtils.ParseEndPointFromString(service.Host);

                if(!_remoteList.Contains(service.Host))
                {
                    newList.Add(address);
                    SyncTransport(address);
                }
                allList.Add(service.Host);
                AddRouter(key, address, newRouter);
            }
            ComparerRemoteList(allList);
            _routerDict = newRouter;
        }

        private void ComparerRemoteList(HashSet<string> newList)
        {
            List<EndPoint> removeList = new List<EndPoint>();
            if (_remoteList.Count > 0)
            {             
                foreach(var address in _remoteList)
                {
                    if (!newList.Contains(address))
                    {
                        removeList.Add(ParseUtils.ParseEndPointFromString(address));
                    }
                }
             
                foreach (var endpoint in removeList)
                {
                    _transportFactory.CloseTransportAsync(endpoint);
                }
            }
            _remoteList = newList;
        }

        private void SyncTransport(EndPoint endPoint)
        {
            Logger.Debug("CreateTransport:{0} ", endPoint);
            _transportFactory.CreateTransport(endPoint);
            
        }

        private void AddRouter(string key, EndPoint address, Dictionary<string, List<EndPoint>> router)
        {
            if (router.ContainsKey(key))
            {
                router[key].Add(address);
            }
            else
            {
                router.Add(key, new List<EndPoint>() {address});
            }
        }

        public RouterPoint GetRouterPoint(AmpMessage message)
        {
            RouterPoint point = new RouterPoint {RoutePointType = RoutePointType.Local};

            string keyService = $"{message.ServiceId}$0";
            string msgKey = $"{message.ServiceId}${message.MessageId}";
            if (_routerDict == null)
            {
                return point;
            }
            if (_routerDict.ContainsKey(msgKey))
            {
                point.RoutePointType = RoutePointType.Remote;
                point.RemoteAddress = ChooseEndPoint(keyService, _routerDict[msgKey]);
                return point;
            }

            if (_routerDict.ContainsKey(keyService))
            {
                point.RoutePointType = RoutePointType.Remote;
                point.RemoteAddress = ChooseEndPoint(keyService, _routerDict[keyService]);
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
        protected virtual EndPoint ChooseEndPoint(string key, List<EndPoint> list)
        {
            int chooseIndex = 0;
            lock (LockObject)
            {
                if (!ChooseRandom.ContainsKey(key))
                {
                    ChooseRandom.Add(key, chooseIndex);
                }
                else
                {
                    chooseIndex = ChooseRandom[key]++;
                    if (chooseIndex >= list.Count)
                    {
                        chooseIndex = 0;
                    }
                    ChooseRandom[key] = chooseIndex;
                }
            }
            return list[chooseIndex];

        }
    }
}
