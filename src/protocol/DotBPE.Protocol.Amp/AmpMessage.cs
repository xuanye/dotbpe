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
            this.UniqueId = IdUtils.NewId();
        }
        public byte Version { get; set; }
        public ushort ServiceId { get; set; }
        public ushort MessageId { get; set; }

        public byte[] Data { get; set; }

        
        public override int Length
        {
            get
            {
               return 10 + (Data != null?Data.Length:0);
            }
        }
    }
}
