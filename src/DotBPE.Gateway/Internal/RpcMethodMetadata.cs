using System;

namespace DotBPE.Gateway.Internal
{
    public sealed class RpcMethodMetadata
    {       
        /// <summary>
        /// Creates a new instance of <see cref="RpcMethodMetadata"/> with the provided service type and method.
        /// </summary>
        /// <param name="serviceType">The implementing service type.</param>
        /// <param name="method">The method representation.</param>
        public RpcMethodMetadata(Type serviceType, IMethod method)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            ServiceType = serviceType;
            Method = method;
        }

        /// <summary>
        /// Gets the implementing service type.
        /// </summary>
        public Type ServiceType { get; }

        /// <summary>
        /// Gets the method representation.
        /// </summary>
        public IMethod Method { get; }
    }
}
