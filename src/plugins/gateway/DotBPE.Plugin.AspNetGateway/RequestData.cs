using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Plugin.AspNetGateway
{
    public class RequestData
    {

        public int ServiceId { get; set; }
        public int MessageId { get; set; }
        public string RawBody { get; set; }
        public Dictionary<string, string> Data { get; set; }

        //TODO:是否需要手机HEADER中的信息？
    }
}
