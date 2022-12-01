// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

namespace DotBPE.Gateway
{
    public class RpcGatewayOption
    {
        public static RpcGatewayOption Default = new RpcGatewayOption();
        public string CodeFieldName { get; set; } = "code";
        public string MessageFieldName { get; set; } = "message";
        public string DataFieldName { get; set; } = "data";
    }
}
