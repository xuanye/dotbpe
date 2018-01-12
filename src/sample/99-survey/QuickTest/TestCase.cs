using DotBPE.Protocol.Amp;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickTest
{
    public class TestCase
    {
        public string Id
        {
            get;set;
        }

        public ushort ServiceId { get; set; }

        public ushort MessageId { get; set; }

        public string Json { get; set; }

        public IMessage Request { get; set; }
        public IMessage Response { get; set; }
    }
}
