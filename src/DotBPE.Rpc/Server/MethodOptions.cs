using System.Collections.Generic;
using System.Linq;

namespace DotBPE.Rpc.Server
{
    public class MethodOptions
    {
        // Fast check for whether the service has any interceptors
        internal bool HasInterceptors { get; }
         private MethodOptions(
             InterceptorCollection interceptors
            )
        {

            Interceptors = interceptors;
            HasInterceptors = interceptors.Count > 0;
        }

         /// <summary>
         /// Get a collection of interceptors to be executed with every call. Interceptors are executed in order.
         /// </summary>
         public IReadOnlyList<InterceptorRegistration> Interceptors { get; }

        public static MethodOptions Create(IEnumerable<RpcServiceOptions> serviceOptions)
        {
            var tempInterceptors = new List<InterceptorRegistration>();
            foreach (var options in serviceOptions.Reverse())
            {
                tempInterceptors.InsertRange(0, options.Interceptors);
            }

            var interceptors = new InterceptorCollection();
            foreach (var interceptor in tempInterceptors)
            {
                interceptors.Add(interceptor);
            }

            return new MethodOptions
            (
                interceptors: interceptors
            );
        }
    }
}
