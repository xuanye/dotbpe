using DotBPE.Rpc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DotBPE.Gateway.Internal
{
    internal class RpcServiceScaner
    {
        private readonly ILogger<RpcServiceScaner> _logger;

        public RpcServiceScaner(IServiceProvider provider,ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<RpcServiceScaner>();

            provider.GetServices(typeof(IRpcServiceMethodProvider<>));
        }

        public void ScanService(string dllPrefix = "*"  , params string[] categories)
        {          

            string basePath = Rpc.Internal.Environment.GetAppBasePath();

            var dllFiles = Directory.GetFiles(string.Concat(basePath, ""), $"{dllPrefix}.dll");


            _logger.LogDebug("dll count={0}", dllFiles.Length);

            List<Assembly> assemblies = new List<Assembly>();
            foreach (var file in dllFiles)
            {

                assemblies.Add(Assembly.LoadFrom(file));
            }

            _logger.LogDebug("assembly count={0}", assemblies.Count);

            foreach (var a in assemblies)
            {
                //Console.WriteLine(a.FullName);
                foreach (var type in a.GetTypes())
                {

                    if (!type.IsInterface)
                        continue;

                    //this._logger.LogInformation(type.FullName);
                    var sAttr = type.GetCustomAttribute<RpcServiceAttribute>();
                    if (sAttr == null)
                        continue;

                    AddRpcService(type, sAttr,categories);
                }
            }
           
        }
        private void AddRpcService(Type type, RpcServiceAttribute sAttr,params string[] categories)
        {
            var methods = type.GetMethods();
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
                        AddHttpServiceRouter(type, m, sAttr, mAttr, rAttr);
                    }
                }
                else
                {
                    if ("default".Equals(rAttr.Category, StringComparison.OrdinalIgnoreCase))
                    {
                        AddHttpServiceRouter(type, m, sAttr, mAttr, rAttr);
                    }
                }
            }
        }

        private void AddHttpServiceRouter(Type type, MethodInfo m, RpcServiceAttribute sAttr, RpcMethodAttribute mAttr, RouterAttribute rAttr)
        {
            throw new NotImplementedException();
        }
    }
}
