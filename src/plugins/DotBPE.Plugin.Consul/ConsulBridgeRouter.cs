using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DotBPE.Rpc;
using DotBPE.Rpc.Logging;
using DotBPE.Rpc.Utils;
using Microsoft.Extensions.Options;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.ServiceRegistry;
using DotBPE.Plugin.Consul.ServiceRegistry;
using Environment = DotBPE.Rpc.Environment;


namespace DotBPE.Plugin.Consul
{
    public class ConsulBridgeRouter<TMessage> : Rpc.DefaultImpls.AbstractBridgeRouter<TMessage> where TMessage:InvokeMessage
    {
        static readonly ILogger Logger = Environment.Logger.ForType<ConsulBridgeRouter<TMessage>>();
        private readonly IServiceDiscoveryProvider _discovery;
        private readonly ServiceDiscoveryOption _options;
        private bool _stop = false;
        private Dictionary<string, List<EndPoint>> _routerDict = null;
        private HashSet<string> _remoteList = new HashSet<string>();
        private static readonly Dictionary<string, int> ChooseRandom = new Dictionary<string, int>();
        private static readonly object LockObject = new object();
        private ITransportFactory<TMessage> _transportFactory;


        public ConsulBridgeRouter(IServiceDiscoveryProvider discovery, ITransportFactory<TMessage> transportFactory, IOptions<ServiceDiscoveryOption> options)
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
            var list = await this._discovery.FindServicesAsync("");
            if (list.Count == 0)
            {
                return;
            }
            var newRouter = new Dictionary<string, List<EndPoint>>();
            var newList = new List<EndPoint>();
            var allList = new HashSet<string>();
            foreach (var service in list)
            {
                string keyService = service.ServiceId + "$0";
                Logger.Debug("Load Service Id= {0},Address = {1}", service.ServiceId, service.FormatAddress);

                var address = ParseUtils.ParseEndPointFromString(service.FormatAddress);

                if (!_remoteList.Contains(service.FormatAddress))
                {
                    newList.Add(address);
                    SyncTransport(address);
                }
                allList.Add(service.FormatAddress);
                AddRouter(keyService, address, newRouter);
            }
            ComparerRemoteList(allList);
            _routerDict = newRouter;
        }

        private void ComparerRemoteList(HashSet<string> newList)
        {
            List<EndPoint> removeList = new List<EndPoint>();
            if (_remoteList.Count > 0)
            {
                foreach (var address in _remoteList)
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
                router.Add(key, new List<EndPoint>() { address });
            }
        }

        
        public override RouterPoint GetRouterPoint(TMessage message)
        {

            RouterPoint point = new RouterPoint { RoutePointType = RoutePointType.Local };

            string keyService = message.ServiceIdentifier;
            string keyMessage = message.MethodIdentifier;

            if (_routerDict == null)
            {
                return point;
            }
            if (_routerDict.ContainsKey(keyMessage))
            {
                point.RoutePointType = RoutePointType.Remote;
                point.RemoteAddress = ChooseEndPoint(keyService, _routerDict[keyMessage]);
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
    }
}
