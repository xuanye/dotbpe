using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Peach.Buffer
{
    public class ByteBufferReader : IBufferReader
    {
        private readonly IByteBuffer _buffer;

        public ByteBufferReader(IByteBuffer buffer)
        {
            this._buffer = buffer;
        }

        public byte ReadByte()
        {
            return this._buffer.ReadByte();
        }

        public void ReadBytes(byte[] dest)
        {
            this._buffer.ReadBytes(dest);
        }

        public char ReadChar()
        {
            return this._buffer.ReadChar();
        }

        public double ReadDouble()
        {
            return this._buffer.ReadDouble();
        }

        public int ReadInt()
        {
            return this._buffer.ReadInt();
        }

        public long ReadLong()
        {
            return this._buffer.ReadLong();
        }

        public short ReadShort()
        {
            return this._buffer.ReadShort();
        }

        public ushort ReadUShort()
        {
            return this._buffer.ReadUnsignedShort();
        }

        public uint ReadUInt()
        {
            return this._buffer.ReadUnsignedInt();
        }

        public int ReadableBytes => this._buffer.ReadableBytes;
    }

   
}
