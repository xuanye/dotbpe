using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Plugin.Gateway
{
    public class RequestData
    {

        public int ServiceId { get; set; }
        public int MessageId { get; set; }
        public string Body { get; set; }
        public Dictionary<string, string> Data { get; set; }

        public bool NeedAuth { get; set; }
    }
}
