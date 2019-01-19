using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Peach.Buffer
{
    internal static class ByteBufferManager
    {
        internal static IBufferWriter CreateBufferWriter(IByteBuffer buffer)
        {
            return new ByteBufferWriter(buffer);
        }

        internal static IBufferReader CreateBufferReader(IByteBuffer input)
        {
            return new ByteBufferReader(input);
        }
    }
}
