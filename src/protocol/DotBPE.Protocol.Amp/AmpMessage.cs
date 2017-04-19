using System;
using System.Collections.Generic;
using System.Text;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Utils;

namespace DotBPE.Protocol.Amp
{
    public class AmpMessage: InvokeMessage
    {
        public AmpMessage()
        {
            this.Version = 0;        
        }
        public byte Version { get; set; }
        public ushort ServiceId { get; set; }
        public ushort MessageId { get; set; }
        public int Sequence { get; set; }
        public byte[] Data { get; set; }

        /// <summary>
        /// 消息标识
        /// </summary>
        public string Id
        {
            get
            {
                return $"{ServiceId}|{MessageId}|{Sequence}";
            }
        }
        public override int Length
        {
            get
            {
               return 14 + (Data != null?Data.Length:0);
            }
        }

        public static AmpMessage CreateRequestMessage(ushort serviceId, ushort messageId)
        {
            AmpMessage message = new AmpMessage()
            {
                ServiceId = serviceId,
                MessageId = messageId,
                InvokeMessageType = InvokeMessageType.Request
            };
            return message;

        }
        public static AmpMessage CreateResponseMessage(ushort serviceId, ushort messageId)
        {
            AmpMessage message = new AmpMessage()
            {
                ServiceId = serviceId,
                MessageId = messageId,
                InvokeMessageType = InvokeMessageType.Response
            };
            return message;
        }
    }
}
