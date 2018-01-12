using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace DotBPE.Rpc.Client
{
    public class ClientProxy
    {
        private static readonly Dictionary<Type, object> cache = new Dictionary<Type, object>();
        private static readonly object lockObj = new object();

        public static T GetClient<T>() where T : class
        {
            Type type = typeof(T);
            if (cache.ContainsKey(type))
            {
                return cache[type] as T;
            }
            else
            {
                T client = ActivatorUtilities.CreateInstance<T>(Rpc.Environment.ServiceProvider);
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
