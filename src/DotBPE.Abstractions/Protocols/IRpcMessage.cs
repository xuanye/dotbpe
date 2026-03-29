// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;

namespace DotBPE.Abstractions.Protocols
{
    /// <summary>
    /// RPC message interface representing the base message structure
    /// </summary>
    public interface IRpcMessage
    {
        /// <summary>
        /// Gets the message type (Request, Response, Notify, etc.)
        /// </summary>
        RpcMessageType MessageType { get; }

        /// <summary>
        /// Gets the length of the message
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Service identifier to locate a service
        /// </summary>
        string ServiceIdentity { get; }

        /// <summary>
        /// Method identifier to locate a method
        /// </summary>
        string MethodIdentifier { get; }

        /// <summary>
        /// Indicates if this is a heartbeat message
        /// </summary>
        bool IsHeartBeat { get; }
    }

    /// <summary>
    /// RPC message type enumeration
    /// </summary>
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
        /// Notify message (one-way, no response expected)
        /// </summary>
        Notify = 3,

        /// <summary>
        /// Invoke without response
        /// </summary>
        OnewayRequest = 4
    }
}
