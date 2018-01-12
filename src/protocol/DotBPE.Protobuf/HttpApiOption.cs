using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Protobuf
{
    public sealed partial class HttpApiOption
    {
        public ushort ServiceId { get; set; }
        public ushort MessageId { get; set; }
    }
}
