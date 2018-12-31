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
            _buffer = buffer;
        }

        public byte ReadByte()
        {
            return _buffer.ReadByte();
        }

        public void ReadBytes(byte[] dest)
        {
            _buffer.ReadBytes(dest);
        }

        public char ReadChar()
        {
            return _buffer.ReadChar();
        }

        public double ReadDouble()
        {
            return _buffer.ReadDouble();
        }

        public int ReadInt()
        {
            return _buffer.ReadInt();
        }

        public long ReadLong()
        {
            return _buffer.ReadLong();
        }

        public short ReadShort()
        {
            return _buffer.ReadShort();
        }

        public ushort ReadUShort()
        {
            return _buffer.ReadUnsignedShort();
        }

        public uint ReadUInt()
        {
            return _buffer.ReadUnsignedInt();
        }

        public int ReadableBytes => _buffer.ReadableBytes;
    }

   
}
