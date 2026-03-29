// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;

namespace DotBPE.Rpc.Protocols
{
    /// <summary>
    /// RPC message interface
    /// </summary>
    /// <remarks>
    /// This type is obsolete. Use <see cref="DotBPE.Abstractions.Protocols.IRpcMessage"/> instead.
    /// </remarks>
    [Obsolete("Use DotBPE.Abstractions.Protocols.IRpcMessage instead")]
    public interface IRpcMessage : DotBPE.Abstractions.Protocols.IRpcMessage
    {
    }

    /// <summary>
    /// RPC message type enumeration
    /// </summary>
    /// <remarks>
    /// This type is obsolete. Use <see cref="DotBPE.Abstractions.Protocols.RpcMessageType"/> instead.
    /// </remarks>
    [Obsolete("Use DotBPE.Abstractions.Protocols.RpcMessageType instead")]
    public enum RpcMessageType : byte
    {
        /// <summary>
        /// Request message (expects response)
        /// </summary>
        Request = 1,
        /// <summary>
        /// Response message
        /// </summary>
        Response = 2,
        /// <summary>
        /// Notify message (one-way response)
        /// </summary>
        Notify = 3,
        /// <summary>
        /// Oneway request (no response expected)
        /// </summary>
        OnewayRequest = 4
    }
}
