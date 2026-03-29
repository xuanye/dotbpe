// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

namespace DotBPE.Abstractions
{
    /// <summary>
    /// Method descriptor interface for RPC service methods
    /// </summary>
    public interface IMethod
    {
        /// <summary>
        /// Gets the group name for the service
        /// </summary>
        string GroupName { get; }

        /// <summary>
        /// Gets the service name
        /// </summary>
        string ServiceName { get; }

        /// <summary>
        /// Gets the method name
        /// </summary>
        string MethodName { get; }

        /// <summary>
        /// Gets the service identifier
        /// </summary>
        int ServiceId { get; }

        /// <summary>
        /// Gets the method identifier
        /// </summary>
        ushort MethodId { get; }

        /// <summary>
        /// Gets the full name (ServiceName.MethodName)
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Gets or sets the default timeout in milliseconds
        /// </summary>
        int DefaultTimeout { get; set; }
    }
}
