using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace DotBPE.Rpc.Client
{
    public class ClientProxy : IClientProxy
    {
        private readonly IServiceProvider _serviceProvider;

        public ClientProxy(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        private static readonly Dictionary<Type, object> cache = new Dictionary<Type, object>();
        private static readonly object lockObj = new object();

        public TService GetService<TService>()
        {
            return this._serviceProvider.GetService<TService>();
        }

        public TClient GetClient<TClient>() where TClient : class, IInvokeClient
        {
            Type type = typeof(TClient);
            if (cache.ContainsKey(type))
            {
                return cache[type] as TClient;
            }
            else
            {
                TClient client = ActivatorUtilities.CreateInstance<TClient>(this._serviceProvider);
                lock (lockObj)
                {
                    if (!cache.ContainsKey(type))
                    {
                        cache.Add(type, client);
                    }
                }
                return client;
            }
        }
    }
}
