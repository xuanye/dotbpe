using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc
{
    public interface ISerializer
    {
        T Deserialize<T>(byte[] data);

        byte[] Serialize<T>(T item);
       
    }
}
