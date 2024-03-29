// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Codec
{
    public enum CodecType : byte
    {
        Protobuf = 0,
        MessagePack = 1,
        JSON = 2,
        Unknown = 9,
    }
}
