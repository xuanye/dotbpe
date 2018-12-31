using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Peach.Buffer
{
    public class ByteBufferWriter : IBufferWriter
    {
        private readonly IByteBuffer _buffer;

        public ByteBufferWriter(IByteBuffer buffer)
        {
            _buffer = buffer;
        }

        public IBufferWriter WriteByte(byte value)
        {
            _buffer.WriteByte(value);
            return this;
        }

        public IBufferWriter WriteBytes(byte[] value)
        {
            _buffer.WriteBytes(value);
            return this;
        }

        public IBufferWriter WriteChar(char value)
        {
            _buffer.WriteChar(value);
            return this;
        }

        public IBufferWriter WriteDouble(double value)
        {
            _buffer.WriteDouble(value);
            return this;
        }

        public IBufferWriter WriteInt(int value)
        {
            _buffer.WriteInt(value);
            return this;
        }

        public IBufferWriter WriteShort(short value)
        {
            _buffer.WriteShort(value);
            return this;
        }

        public IBufferWriter WriteUInt(uint value)
        {
            _buffer.WriteIntLE((int)value);
            return this;
        }

        public IBufferWriter WriteUShort(ushort value)
        {
            _buffer.WriteUnsignedShort(value);
            return this;
        }

        public IBufferWriter WriteLong(long value)
        {
            _buffer.WriteLong(value);
            return this;
        }
    }
}
