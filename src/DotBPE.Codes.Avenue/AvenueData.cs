using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Codes.Avenue
{
    public class AvenueData
    {
        public AvenueData()
        {

        }
        public AvenueData(
            int flag,
            int version,
            int serviceId,
            int msgId,
            int sequence,
            int mustReach,
            int encoding,
            int format,
            IByteBuffer xhead,
            IByteBuffer body
            )
        {
            this.Version = version;
            this.Flag = flag;
            this.Sequence = sequence;
            this.ServiceId = serviceId;
            this.MsgId = msgId;
            this.MustReach = mustReach;
            this.Encoding = encoding;
            this.Format = format;
            this.XHead = xhead;
            this.Body = body;
        }

        public int Flag { get; set; }
        public int ServiceId { get; set; }
        public int MsgId { get; set; }
        public int Sequence { get; set; }
        public int MustReach { get; set; }
        public int Encoding { get; set; }
        public int Format { get; set; }
        public IByteBuffer XHead { get; set; } // may be changed by sos
        public IByteBuffer Body { get; set; }

        public int Version { get; set; }

        public int PackageLength {
            get
            {
                return this.HeadLength + (this.Body != null ? this.Body.Capacity : 0);
            }
        }
        public int HeadLength
        {
            get {
                return Constants.STANDARD_HEADLEN + (this.XHead != null ? this.XHead.Capacity : 0);
            }
        }
    }
}
