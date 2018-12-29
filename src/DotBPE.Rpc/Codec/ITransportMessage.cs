using Peach.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Codec
{
    public interface ITransportMessage
    {
        CodecType CodecType { get; }

        byte[] Encode<T>(T message);

        T Decode<T>(byte[] buffer);
    }
}
