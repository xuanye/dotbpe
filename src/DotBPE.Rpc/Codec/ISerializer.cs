using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc
{
    public interface ISerializer
    {
        T Deserialize<T>(byte[] data);
        byte[] Serialize<T>(T item);


        object Deserialize(byte[] data,Type type);


        byte[] Serialize(object item);

        byte CodecType { get; }

    }
}
