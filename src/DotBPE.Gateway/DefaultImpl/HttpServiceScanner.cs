using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotBPE.Gateway
{
    public class HttpServiceScanner:IHttpServiceScanner
    {
        private readonly IClientProxy _proxy;
        private readonly IServiceProvider _provider;
        private readonly ILogger<HttpServiceScanner> _logger;

        private readonly MethodInfo _proxyCreate;

        public HttpServiceScanner(
            IClientProxy proxy,
            IServiceProvider provider,
            ILogger<HttpServiceScanner> logger)
        {
            this._proxy = proxy;

            _proxyCreate = proxy.GetType().GetMethod("Create");

            this._provider = provider;
            this._logger = logger;
        }
        public HttpRouteOptions Scan(string dllPrefix="*",params string[] categories)
        {
            HttpRouteOptions options = new HttpRouteOptions();

            string basePath = Rpc.Internal.Environment.GetAppBasePath();

            var dllFiles = Directory.GetFiles(string.Concat(basePath, ""), $"{dllPrefix}.dll");

            List<Assembly> assemblies = new List<Assembly>();
            foreach (var file in dllFiles)
            {
                assemblies.Add(Assembly.LoadFrom(file));
            }

            foreach (var a in assemblies)
            {
                //Console.WriteLine(a.FullName);
                foreach (var type in a.GetTypes())
                {
                    if (!type.IsInterface)
                       continue;

                    var sAttr = type.GetCustomAttribute<RpcServiceAttribute>();
                    if (sAttr == null)
                        continue;

                    AddRpcService(type, sAttr, options,categories);
                }
            }
            return options;
        }

        private void AddRpcService(Type type, RpcServiceAttribute sAttr, HttpRouteOptions options,params string[] categories)
        {
            var methods = type.GetMethods(BindingFlags.Public);
            foreach (var m in methods)
            {
                var mAttr = m.GetCustomAttribute<RpcMethodAttribute>();
                if (mAttr == null)
                    continue;

                var rAttr = m.GetCustomAttribute<RouterAttribute>();
                if (rAttr == null)
                    continue;

                if (categories != null && categories.Any())
                {
                    if (categories.Contains(rAttr.Category))
                    {
                        AddHttpServiceRouter(type,m, sAttr, mAttr, rAttr, options);
                    }
                }
                else
                {
                    AddHttpServiceRouter(type,m, sAttr, mAttr, rAttr, options);
                }
            }
        }

        private void AddHttpServiceRouter(Type type,MethodInfo m, RpcServiceAttribute sAttr, RpcMethodAttribute mAttr,
            RouterAttribute rAttr, HttpRouteOptions options)
        {
            var item = new RouteItem
            {
                Path = rAttr.Path,
                AcceptVerb = rAttr.AcceptVerb,
                Category = rAttr.Category,
                InvokeMethod = m,
                InvokeService = this._proxyCreate.MakeGenericMethod(type).Invoke(this._proxy, new object[0]),
                MessageId = mAttr.MessageId,
                ServiceId = sAttr.ServiceId
            };

            if (rAttr.PluginType != null )
            {
                item.Plugin = ActivatorUtilities.CreateInstance(this._provider, rAttr.PluginType) as IHttpPlugin;
            }
            options.Items.Add(item);

            _logger.LogInformation("url:{0},verb:{1},service:{2},method:{3}",
                item.Path,item.AcceptVerb,type.Name.Split('.').Last(),m.Name);

        }
    }
}
