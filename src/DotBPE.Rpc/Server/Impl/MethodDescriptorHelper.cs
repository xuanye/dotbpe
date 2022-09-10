using System.Collections.Concurrent;
using System.Reflection;

namespace DotBPE.Rpc.Server
{
    internal class MethodDescriptorHelper
    {

        private static readonly ConcurrentDictionary<string, MethodInfo> _methodCache =
            new ConcurrentDictionary<string, MethodInfo>();

        public static bool TryAdd(string key, MethodInfo method)
        {
            return _methodCache.TryAdd(key, method);
        }
        public static bool TryGetValue(string key,out MethodInfo method)
        {
            return _methodCache.TryGetValue(key, out method);
        }

    }
}
