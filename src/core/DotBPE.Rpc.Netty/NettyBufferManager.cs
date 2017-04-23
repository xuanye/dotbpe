using System;
using System.Collections.Generic;
using System.Text;
using DotBPE.Rpc.Codes;
using DotNetty.Buffers;

namespace DotBPE.Rpc.Netty
{
    public class NettyBufferManager
    {
        internal static IBufferWriter CreateBufferWriter(IByteBuffer buffer)
        {
            return new NettyByteBufferWriter(buffer);
        }

        internal static IBufferReader CreateBufferReader(IByteBuffer input)
        {
            return new NettyByteBufferReader(input);
        }
    }

    public class NettyByteBufferReader : IBufferReader
    {
        private readonly IByteBuffer _buffer;
        public NettyByteBufferReader(IByteBuffer buffer)
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

        public int ReadableBytes => _buffer.ReadableBytes;
    }
    public class NettyByteBufferWriter : IBufferWriter
    {
        private readonly IByteBuffer _buffer;
        public NettyByteBufferWriter(IByteBuffer buffer)
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
            this._buffer.WriteUnsignedInt(value);
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
