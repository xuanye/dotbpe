using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace DotBPE.Rpc.Server
{
    public class InterceptorRegistration
    {
        internal readonly object[] _args;

        private IRpcInterceptorActivator _interceptorActivator;

        private ObjectFactory _factory;

        /// <summary>
        /// Get the type of the interceptor.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Get the arguments used to create the interceptor.
        /// </summary>
        public IReadOnlyList<object> Arguments => _args;

        internal InterceptorRegistration(Type type, object[] arguments)
        {
            if (arguments == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            if (arguments.Any(t => t == null))
            {
                throw new ArgumentException(
                    "Interceptor arguments contains a null value. Null interceptor arguments are not supported.",
                    nameof(arguments));
            }

            Type = type ?? throw new ArgumentNullException(nameof(type));
            _args = arguments;
        }

        internal IRpcInterceptorActivator GetActivator(IServiceProvider serviceProvider)
        {
            return _interceptorActivator ?? (_interceptorActivator =
                (IRpcInterceptorActivator) serviceProvider.GetRequiredService(
                    typeof(IRpcInterceptorActivator<>).MakeGenericType(Type)));
        }

        internal ObjectFactory GetFactory()
        {
            return _factory ?? (_factory =
                ActivatorUtilities.CreateFactory(Type, _args.Select(a => a.GetType()).ToArray()));
        }

    }
}
