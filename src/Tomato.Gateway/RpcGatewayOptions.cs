using System;
using System.Collections.Generic;
using System.Text;

namespace Tomato.Gateway
{
    public class RpcGatewayOptions
    {
        public string WrapperCodeFieldName { get; set; } = "code";
        public string WrapperMessageFieldName { get; set; } = "message";
        public string WrapperDataFieldName { get; set; } = "data";
    }
}
