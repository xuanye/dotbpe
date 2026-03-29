// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System.Net;

namespace DotBPE.Abstractions
{
    /// <summary>
    /// RPC context interface providing access to connection information
    /// </summary>
    public interface IRpcContext
    {
        /// <summary>
        /// Gets the remote endpoint address
        /// </summary>
        IPEndPoint RemoteAddress { get; }

        /// <summary>
        /// Gets the local endpoint address
        /// </summary>
        IPEndPoint LocalAddress { get; }
    }
}
