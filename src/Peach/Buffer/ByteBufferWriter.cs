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
            this._buffer = buffer;
        }

        public IBufferWriter WriteByte(byte value)
        {
            this._buffer.WriteByte(value);
            return this;
        }

        public IBufferWriter WriteBytes(byte[] value)
        {
            this._buffer.WriteBytes(value);
            return this;
        }

        public IBufferWriter WriteChar(char value)
        {
            this._buffer.WriteChar(value);
            return this;
        }

        public IBufferWriter WriteDouble(double value)
        {
            this._buffer.WriteDouble(value);
            return this;
        }

        public IBufferWriter WriteInt(int value)
        {
            this._buffer.WriteInt(value);
            return this;
        }

        public IBufferWriter WriteShort(short value)
        {
            this._buffer.WriteShort(value);
            return this;
        }

        public IBufferWriter WriteUInt(uint value)
        {
            this._buffer.WriteIntLE((int)value);
            return this;
        }

        public IBufferWriter WriteUShort(ushort value)
        {
            this._buffer.WriteUnsignedShort(value);
            return this;
        }

        public IBufferWriter WriteLong(long value)
        {
            this._buffer.WriteLong(value);
            return this;
        }
    }
}
