using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Gateway
{
    public class RpcGatewayOption
    {
        public string CodeFieldName { get; set; } = "code";
        public string MessageFieldName { get; set; } = "message";
        public string DataFieldName { get; set; } = "data";
    }
}
