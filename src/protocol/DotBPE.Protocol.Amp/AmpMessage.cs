using DotBPE.Rpc.Codes;

namespace DotBPE.Protocol.Amp
{
    public class AmpMessage : InvokeMessage
    {
        public const int HEAD_LENGTH = 18;

        public AmpMessage()
        {
            this.Version = 0;
        }

        public byte Version { get; set; }
        public ushort ServiceId { get; set; }
        public ushort MessageId { get; set; }
        public int Sequence { get; set; }
        public int Code { get; set; }
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
                return HEAD_LENGTH + (Data != null ? Data.Length : 0);
            }
        }

        public override string ServiceIdentifier => $"{ServiceId}$0";

        public override string MethodIdentifier => $"{ServiceId}${MessageId}";

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

        public static AmpMessage CreateResponseMessage(string requestId)
        {
            var data = requestId.Split('|');
            AmpMessage message = new AmpMessage()
            {
                ServiceId = ushort.Parse(data[0]),
                MessageId = ushort.Parse(data[1]),
                InvokeMessageType = InvokeMessageType.Response
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
