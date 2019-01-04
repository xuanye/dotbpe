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
            this._routeOptions = routeOptions?.Value?? new RouterPointOptions();
            this._policy = policy;
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
            if(this._routeOptions.Messages !=null && this._routeOptions.Messages.Any())
            {
                foreach(var cfg in this._routeOptions.Messages)
                {
                    var remoteLst  = EndPointParser.ParseEndPointListFromString(cfg.RemoteAddress);
                    if (remoteLst.Any())
                    {
                        AddRouter($"{cfg.ServiceId}${cfg.MessageId}", cfg.Weight, remoteLst);

                    }
                }
            }
        }
        private void InitializeServices()
        {
            if (this._routeOptions.Services != null && this._routeOptions.Services.Any())
            {
                foreach (var cfg in this._routeOptions.Services)
                {
                    var remoteLst = EndPointParser.ParseEndPointListFromString(cfg.RemoteAddress);
                    if (remoteLst.Any())
                    {
                        AddRouter($"{cfg.ServiceId}$0",cfg.Weight, remoteLst);
                    }
                }
            }
        }
        private void InitializeCategory()
        {
            if (this._routeOptions.Categories != null && this._routeOptions.Categories.Any())
            {
                foreach (var cfg in this._routeOptions.Categories)
                {
                    var remoteLst = EndPointParser.ParseEndPointListFromString(cfg.RemoteAddress);
                    if (remoteLst.Any())
                    {
                        AddRouter($"{cfg.Category}",cfg.Weight, remoteLst);
                    }
                }
            }
        }





        private void AddRouter(string key,int weight, List<EndPoint> remoteAddress)
        {
            var ls = remoteAddress.ConvertAll<IRouterPoint>(
                                x => new RouterPoint
                                {
                                    RemoteAddress = x,
                                    RoutePointType = RoutePointType.Remote,
                                    Weight = weight
                                });

            if (this.SERVICE_CACHE.ContainsKey(key))
            {
                this.SERVICE_CACHE[key].AddRange(ls);
            }
            else
            {
                this.SERVICE_CACHE.Add(key, ls);
            }
            //order by weight
            this.SERVICE_CACHE[key].Sort((x, y) => x.Weight > y.Weight ? 1 : 0);
        }


        public IRouterPoint FindRouterPoint(string servicePath)
        {
            IRouterPoint point = new RouterPoint {  RoutePointType = RoutePointType.Local };


            var parts = servicePath.Split(';');

            var keys = parts[0].Split('$');
            var keyService = keys[0]+"$0";
            var keyMessage = servicePath;


            if (this.SERVICE_CACHE.ContainsKey(keyMessage))
            {
                point = SelectEndPoint(keyMessage, this.SERVICE_CACHE[keyMessage]);
                return point;
            }

            if (this.SERVICE_CACHE.ContainsKey(keyService))
            {
                point = SelectEndPoint(keyService, this.SERVICE_CACHE[keyService]);
                return point;
            }


            var keyCategory = parts.Length == 2 ? parts[1] : "default";
            if (string.IsNullOrEmpty(keyCategory))
            {
                keyCategory = "default";
            }

            //默认配置
            if (this.SERVICE_CACHE.ContainsKey(keyCategory))
            {
                point = SelectEndPoint(keyCategory, this.SERVICE_CACHE[keyCategory]);
                return point;
            }

            return point;
        }

        private IRouterPoint SelectEndPoint(string serviceKey,List<IRouterPoint> remoteAddresses)
        {
            return this._policy.Select(serviceKey, remoteAddresses);
        }


    }
}
