// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Protocol.Codec;
using System;

namespace DotBPE.Protocol
{
    /// <summary>
    /// Protocol Message: A Message Protocol
    /// </summary>
    public class AmpMessage : Peach.Messaging.IMessage, Abstractions.Protocols.IRpcMessage
    {
        /// <summary>
        /// The first version is 18 bytes header fixed length
        /// </summary>
        public const int VERSION_0_HEAD_LENGTH = 18;

        /// <summary>
        /// The enhanced version has a fixed length of 21 byte headers
        /// </summary>
        public const int VERSION_1_HEAD_LENGTH = 21;


        /// <summary>
        /// Status Code
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 0=Protobuf 1=MessagePack 2 = JSON
        /// </summary>
        public CodecType CodecType { get; set; }

        /// <summary>
        /// message data buffer
        /// </summary>
        public byte[] Data { get; set; }

        public bool IsHeartBeat => ServiceId == 0 && MessageId == 0;

        public int Length
        {
            get
            {
                var hl = Version == 0 ? VERSION_0_HEAD_LENGTH : VERSION_1_HEAD_LENGTH;
                if (Data == null)
                {
                    return hl;
                }

                return hl + Data.Length;
            }
        }

        /// <summary>
        /// Unique Id
        /// </summary>
        public string Id => $"{ServiceId}|{MessageId}|{Sequence}";


        /// <summary>
        ///  Unique message number of the calling service Determines which method
        /// </summary>
        public ushort MessageId { get; set; }


        public string MethodIdentifier => $"{ServiceId}.{MessageId}";

        /// <summary>
        /// serial number
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// The unique service number of the service, Determine which service
        /// </summary>
        public int ServiceId { get; set; }

        public string ServiceIdentity => $"{ServiceId}.0";

        public string FriendlyServiceName { get; set; }
        public string ServiceGroupName { get; set; } = "default";

        public string RoutePath => $"{ServiceGroupName}.{MethodIdentifier}";

        /// <summary>
        /// Version 0/1
        /// </summary>
        public byte Version { get; set; }

        public Abstractions.Protocols.RpcMessageType MessageType { get; set; }

        public static AmpMessage CreateRequestMessage(int serviceId, ushort messageId, CodecType codecType,
            bool oneway = false)
        {
            var message = new AmpMessage
            {
                ServiceId = serviceId,
                MessageId = messageId,
                Version = 1,
                CodecType = codecType,
                MessageType = oneway ? Abstractions.Protocols.RpcMessageType.OnewayRequest : Abstractions.Protocols.RpcMessageType.Request
            };
            return message;
        }
        public static AmpMessage CreateResponseMessage(string requestId)
        {
            var data = requestId.Split('|');
            var message = new AmpMessage
            {
                ServiceId = int.Parse(data[0]),
                MessageId = ushort.Parse(data[1]),
                Version = 1,
                CodecType = 0,
                MessageType = Abstractions.Protocols.RpcMessageType.Response
            };
            return message;
        }
        public static AmpMessage CreateResponseMessage(AmpMessage request)
        {
            var message = new AmpMessage
            {
                ServiceId = request.ServiceId,
                MessageId = request.MessageId,
                Version = 1,
                Sequence = request.Sequence,
                CodecType = request.CodecType,
                MessageType = Abstractions.Protocols.RpcMessageType.Response
            };
            return message;
        }

        public static AmpMessage CreateHeartBeatMessage(CodecType codecType)
        {
            var message = new AmpMessage
            {
                ServiceId = 0,
                MessageId = 0,
                Version = 1,
                Sequence = 0,
                CodecType = codecType,
                MessageType = Abstractions.Protocols.RpcMessageType.Request
            };
            return message;
        }
    }
}
