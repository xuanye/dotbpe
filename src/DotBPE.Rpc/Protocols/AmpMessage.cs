// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;

namespace DotBPE.Rpc.Protocols
{
    /// <summary>
    /// Protocol Message: A Message Protocol
    /// </summary>
    /// <remarks>
    /// This type is obsolete. Use <see cref="DotBPE.Protocol.AmpMessage"/> instead.
    /// </remarks>
    [Obsolete("Use DotBPE.Protocol.AmpMessage instead")]
    public class AmpMessage : DotBPE.Protocol.AmpMessage
    {
        /// <summary>
        /// Creates a new instance of AmpMessage
        /// </summary>
        public AmpMessage() : base()
        {
        }

        /// <summary>
        /// Creates a request message
        /// </summary>
        public new static AmpMessage CreateRequestMessage(int serviceId, ushort messageId, Codec.CodecType codecType,
            bool oneway = false)
        {
            return (AmpMessage)DotBPE.Protocol.AmpMessage.CreateRequestMessage(
                serviceId, messageId, (DotBPE.Protocol.Codec.CodecType)codecType, oneway);
        }

        /// <summary>
        /// Creates a response message from request id
        /// </summary>
        public new static AmpMessage CreateResponseMessage(string requestId)
        {
            return (AmpMessage)DotBPE.Protocol.AmpMessage.CreateResponseMessage(requestId);
        }

        /// <summary>
        /// Creates a response message from request
        /// </summary>
        public new static AmpMessage CreateResponseMessage(AmpMessage request)
        {
            return (AmpMessage)DotBPE.Protocol.AmpMessage.CreateResponseMessage(request);
        }

        /// <summary>
        /// Creates a heartbeat message
        /// </summary>
        public new static AmpMessage CreateHeartBeatMessage(Codec.CodecType codecType)
        {
            return (AmpMessage)DotBPE.Protocol.AmpMessage.CreateHeartBeatMessage(
                (DotBPE.Protocol.Codec.CodecType)codecType);
        }

        /// <summary>
        /// Status Code
        /// </summary>
        public new int Code
        {
            get => base.Code;
            set => base.Code = value;
        }

        /// <summary>
        /// 0=Protobuf 1=MessagePack 2 = JSON
        /// </summary>
        public new Codec.CodecType CodecType
        {
            get => (Codec.CodecType)base.CodecType;
            set => base.CodecType = (DotBPE.Protocol.Codec.CodecType)value;
        }

        /// <summary>
        /// message data buffer
        /// </summary>
        public new byte[] Data
        {
            get => base.Data;
            set => base.Data = value;
        }

        /// <summary>
        /// Unique message number of the calling service Determines which method
        /// </summary>
        public new ushort MessageId
        {
            get => base.MessageId;
            set => base.MessageId = value;
        }

        /// <summary>
        /// serial number
        /// </summary>
        public new int Sequence
        {
            get => base.Sequence;
            set => base.Sequence = value;
        }

        /// <summary>
        /// The unique service number of the service, Determine which service
        /// </summary>
        public new int ServiceId
        {
            get => base.ServiceId;
            set => base.ServiceId = value;
        }

        /// <summary>
        /// Version 0/1
        /// </summary>
        public new byte Version
        {
            get => base.Version;
            set => base.Version = value;
        }

        /// <summary>
        /// Message type
        /// </summary>
        public new RpcMessageType MessageType
        {
            get => (RpcMessageType)base.MessageType;
            set => base.MessageType = (DotBPE.Abstractions.Protocols.RpcMessageType)value;
        }
    }
}
