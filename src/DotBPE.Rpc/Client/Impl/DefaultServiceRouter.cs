// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Utils;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    public class DefaultServiceRouter : IServiceRouter
    {

        private static readonly IRouterPoint LocalRoutePoint = new RouterPoint { RoutePointType = RoutePointType.Local };


        private readonly RouterPointOptions _routeOptions;
        private readonly IRoutingPolicy _policy;
        private readonly Dictionary<string, List<IRouterPoint>> _routeCache = new Dictionary<string, List<IRouterPoint>>();

        public DefaultServiceRouter(
            IOptions<RouterPointOptions> routeOptions,
            IRoutingPolicy policy)
        {
            _routeOptions = routeOptions.Value ?? new RouterPointOptions();
            _policy = policy;
            Initialize();
        }

        public Task<IRouterPoint> FindRouterPoint(string servicePath)
        {
            var parts = servicePath.Split('.');
            var keyService = $"{parts[1]}.0";
            var keyMessage = $"{parts[1]}.{parts[2]}";

            if (_routeCache.TryGetValue(keyMessage, out var routerPoints))
            {
                return Task.FromResult(SelectEndPoint(keyMessage, routerPoints));
            }
            else if (_routeCache.TryGetValue(keyService, out routerPoints))
            {
                Task.FromResult(SelectEndPoint(keyService, routerPoints));
            }

            var keyCategory = parts[0];
            if (string.IsNullOrEmpty(keyCategory))
            {
                keyCategory = "default";
            }

            if (_routeCache.TryGetValue(keyCategory, out routerPoints))
            {
                return Task.FromResult(SelectEndPoint(keyCategory, routerPoints));
            }

            return Task.FromResult(LocalRoutePoint);
        }


        private void Initialize()
        {
            InitializeMessages();
            InitializeServices();
            InitializeCategory();
        }

        private void InitializeMessages()
        {
            if (_routeOptions.Messages != null && _routeOptions.Messages.Any())
            {
                foreach (var cfg in _routeOptions.Messages)
                {
                    var remoteLst = EndPointParser.ParseEndPointListFromString(cfg.RemoteAddress!);
                    if (remoteLst.Count > 0)
                    {
                        AddRouter($"{cfg.ServiceId}.{cfg.MessageId}", cfg.Weight, remoteLst);

                    }
                }
            }
        }
        private void InitializeServices()
        {
            if (_routeOptions.Services != null && _routeOptions.Services.Any())
            {
                foreach (var cfg in _routeOptions.Services)
                {
                    var remoteLst = EndPointParser.ParseEndPointListFromString(cfg.RemoteAddress!);
                    if (remoteLst.Count > 0)
                    {
                        AddRouter($"{cfg.ServiceId}.0", cfg.Weight, remoteLst);
                    }
                }
            }
        }
        private void InitializeCategory()
        {
            if (_routeOptions.Categories != null && _routeOptions.Categories.Count > 0)
            {
                foreach (var cfg in _routeOptions.Categories)
                {
                    var remoteLst = EndPointParser.ParseEndPointListFromString(cfg.RemoteAddress!);
                    if (remoteLst.Count > 0)
                    {
                        AddRouter($"{cfg.GroupName}", cfg.Weight, remoteLst);
                    }
                }
            }
        }





        private void AddRouter(string key, int weight, List<IPEndPoint> remoteAddress)
        {
            var ls = remoteAddress.ConvertAll<IRouterPoint>(
                                x => new RouterPoint
                                {
                                    RemoteAddress = x,
                                    RoutePointType = RoutePointType.Remote,
                                    Weight = weight
                                });

            if (_routeCache.ContainsKey(key))
            {
                _routeCache[key].AddRange(ls);
            }
            else
            {
                _routeCache.Add(key, ls);
            }
            //order by weight
            _routeCache[key].Sort((x, y) => x.Weight > y.Weight ? 1 : 0);
        }
        private IRouterPoint SelectEndPoint(string serviceKey, List<IRouterPoint> remoteAddresses)
        {
            return _policy.Select(serviceKey, remoteAddresses);
        }

    }
}
