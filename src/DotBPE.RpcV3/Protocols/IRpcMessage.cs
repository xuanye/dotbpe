// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Protocols
{
    public interface IRpcMessage
    {
        RpcMessageType MessageType { get; }

        int Length { get; }


        /// <summary>
        /// Service identifier to locate a service
        /// </summary>
        string ServiceIdentity { get; }

        /// <summary>
        /// Method identifier to locate a method
        /// </summary>
        string MethodIdentifier { get; }

        bool IsHeartBeat { get; }
    }

    public enum RpcMessageType : byte
    {
        Request = 1,
        Response = 2,
        Notify = 3, //Oneway Response
        OnewayRequest = 4 //Oneway Request
    }
}