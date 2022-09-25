// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    public class DefaultServiceRouter : IServiceRouter
    {
        private readonly RouterPointOptions _routeOptions;
        private readonly IRouterPolicy _policy;

        public DefaultServiceRouter(
            IOptions<RouterPointOptions> routeOptions,
            IRouterPolicy policy)
        {
            _routeOptions = routeOptions.Value ?? new RouterPointOptions();
            _policy = policy;
            Initialize();
        }

        public Task<IRouterPoint> FindRouterPoint(string servicePath)
        {
            throw new NotImplementedException();
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
                    var remoteLst = EndPointParser.ParseEndPointListFromString(cfg.RemoteAddress);
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
                    var remoteLst = EndPointParser.ParseEndPointListFromString(cfg.RemoteAddress);
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
                    var remoteLst = EndPointParser.ParseEndPointListFromString(cfg.RemoteAddress);
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

            if (SERVICE_CACHE.ContainsKey(key))
            {
                SERVICE_CACHE[key].AddRange(ls);
            }
            else
            {
                SERVICE_CACHE.Add(key, ls);
            }
            //order by weight
            this.SERVICE_CACHE[key].Sort((x, y) => x.Weight > y.Weight ? 1 : 0);
        }


    }
}
