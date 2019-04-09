using DotBPE.Rpc.Config;
using DotBPE.Rpc.Internal;
using DotBPE.Rpc.Protocol;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    /// <summary>
    /// local config base router
    /// </summary>
    public class DefaultServiceRouter : IServiceRouter
    {
        private readonly RouterPointOptions _routeOptions;
        private readonly IRouterPolicy _policy;
        private readonly Dictionary<string, List<IRouterPoint>> SERVICE_CACHE = new Dictionary<string, List<IRouterPoint>>();



        public DefaultServiceRouter(
            IOptions<RouterPointOptions> routeOptions,
            IRouterPolicy policy
            )
        {
            _routeOptions = routeOptions?.Value?? new RouterPointOptions();
            _policy = policy;
            Initialize();
        }

        private void Initialize()
        {
            InitializeMessages();
            InitializeServices();
            InitializeCategory();
        }

        private void InitializeMessages()
        {
            if(_routeOptions.Messages !=null && _routeOptions.Messages.Any())
            {
                foreach(var cfg in this._routeOptions.Messages)
                {
                    var remoteLst  = EndPointParser.ParseEndPointListFromString(cfg.RemoteAddress);
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
                foreach (var cfg in this._routeOptions.Services)
                {
                    var remoteLst = EndPointParser.ParseEndPointListFromString(cfg.RemoteAddress);
                    if (remoteLst.Count > 0)
                    {
                        AddRouter($"{cfg.ServiceId}.0",cfg.Weight, remoteLst);
                    }
                }
            }
        }
        private void InitializeCategory()
        {
            if (_routeOptions.Categories != null && _routeOptions.Categories.Count > 0)
            {
                foreach (var cfg in this._routeOptions.Categories)
                {
                    var remoteLst = EndPointParser.ParseEndPointListFromString(cfg.RemoteAddress);
                    if (remoteLst.Count > 0)
                    {
                        AddRouter($"{cfg.GroupName}",cfg.Weight, remoteLst);
                    }
                }
            }
        }





        private void AddRouter(string key,int weight, List<IPEndPoint> remoteAddress)
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


        public Task<IRouterPoint> FindRouterPoint(string servicePath)
        {
            IRouterPoint point = new RouterPoint {  RoutePointType = RoutePointType.Local };


            var parts = servicePath.Split('.');


            var keyService = $"{parts[1]}.0";
            var keyMessage = $"{parts[1]}.{parts[2]}";


            if (SERVICE_CACHE.ContainsKey(keyMessage))
            {
                point = SelectEndPoint(keyMessage, SERVICE_CACHE[keyMessage]);
                return Task.FromResult(point);
            }

            if (SERVICE_CACHE.ContainsKey(keyService))
            {
                point = SelectEndPoint(keyService, SERVICE_CACHE[keyService]);
                return Task.FromResult(point);
            }


            var keyCategory = parts[0];
            if (string.IsNullOrEmpty(keyCategory))
            {
                keyCategory = "default";
            }

            //默认配置
            if (SERVICE_CACHE.ContainsKey(keyCategory))
            {
                point = SelectEndPoint(keyCategory, SERVICE_CACHE[keyCategory]);
                return Task.FromResult(point);
            }

            return Task.FromResult(point);
        }

        private IRouterPoint SelectEndPoint(string serviceKey,List<IRouterPoint> remoteAddresses)
        {
            return _policy.Select(serviceKey, remoteAddresses);
        }


    }
}
