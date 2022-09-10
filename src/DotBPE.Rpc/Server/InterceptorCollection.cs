using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Type = System.Type;

namespace DotBPE.Rpc.Server
{
    /// <summary>
    /// Represents the pipeline of interceptors to be invoked when processing a gRPC call.
    /// </summary>
    public class InterceptorCollection : Collection<InterceptorRegistration>
    {
        /// <summary>
        /// Add an interceptor to the end of the pipeline.
        /// </summary>
        /// <typeparam name="TInterceptor">The interceptor type.</typeparam>
        /// <param name="args">The list of arguments to pass to the interceptor constructor when creating an instance.</param>
        public void Add<TInterceptor> (params object[] args) where TInterceptor : Interceptor
        {
            Add (typeof(TInterceptor), args);
        }

        /// <summary>
        /// Add an interceptor to the end of the pipeline.
        /// </summary>
        /// <param name="interceptorType">The interceptor type.</param>
        /// <param name="args">The list of arguments to pass to the interceptor constructor when creating an instance.</param>
        public void Add (Type interceptorType, params object[] args)
        {
            if (interceptorType == null) {
                throw new ArgumentNullException (nameof(interceptorType));
            }
            if (!interceptorType.IsSubclassOf (typeof(Interceptor))) {
                throw new ArgumentException ("Type must inherit from " + typeof(Interceptor).FullName + ".", "interceptorType");
            }
            Add (new InterceptorRegistration (interceptorType, args));
        }

        /// <summary>
        /// Append a set of interceptor registrations to the end of the pipeline.
        /// </summary>
        /// <param name="registrations">The set of interceptor registrations to add.</param>
        internal void AddRange (IEnumerable<InterceptorRegistration> registrations)
        {
            if (registrations == null) {
                throw new ArgumentNullException ("registrations");
            }
            foreach (InterceptorRegistration registration in registrations) {
                Add (registration);
            }
        }
    }
}
