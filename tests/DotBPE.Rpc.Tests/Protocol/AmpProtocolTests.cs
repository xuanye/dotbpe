using System;
using System.IO;
using System.Runtime.CompilerServices;
using DotBPE.Rpc.Codec;
using DotBPE.Rpc.Protocol;
using Peach.Buffer;
using Xunit;

namespace DotBPE.Rpc.Tests.Protocol
{
    public class AmpProtocolTests
    {

        [Fact]
        public void PackAndUnPackTest()
        {
            AmpMessage src = AmpMessage.CreateRequestMessage(1,1);
            src.Version = 1;
            src.Code = 100;
            src.Sequence = 11;
            src.CodecType = CodecType.MessagePack;
            src.Data = new byte[]{1,2,3,4,5,6,7,8,9,0};

            AmpProtocol protocol = new AmpProtocol();

            MemoryStream stream = new MemoryStream();

            var writer = new MemoryBufferWriter(stream);

            protocol.Pack(writer,src);

            Assert.Equal(AmpMessage.VERSION_1_HEAD_LENGTH+10,stream.Length);

            var reader = new MemoryBufferReader(stream);

            var dist = protocol.Parse(reader);

            Assert.NotNull(dist);

            Assert.Equal(src.Id,dist.Id);
            Assert.Equal(src.Version,dist.Version);
            Assert.Equal(src.Code,dist.Code);
            Assert.Equal(src.Sequence,dist.Sequence);
            Assert.Equal(src.CodecType,dist.CodecType);
            Assert.Equal(src.Data.Length,dist.Data.Length);
        }


        public class MemoryBufferWriter:IBufferWriter
        {
            private readonly BinaryWriter  _writer;

            public MemoryBufferWriter(MemoryStream stream)
            {
                stream.Position = 0;
                this._writer = new BinaryWriter(stream);
            }

            public IBufferWriter WriteInt(int value)
            {
                this._writer.Write(value);
                return this;
            }

            public IBufferWriter WriteByte(byte value)
            {
                this._writer.Write(value);
                return this;
            }

            public IBufferWriter WriteLong(long value)
            {
                this._writer.Write(value);
                return this;
            }

            public IBufferWriter WriteDouble(double value)
            {
                this._writer.Write(value);
                return this;
            }

            public IBufferWriter WriteBytes(byte[] value)
            {
                this._writer.Write(value);
                return this;
            }

            public IBufferWriter WriteChar(char value)
            {
                this._writer.Write(value);
                return this;
            }

            public IBufferWriter WriteShort(short value)
            {
                this._writer.Write(value);
                return this;
            }

            public IBufferWriter WriteUInt(uint value)
            {
                this._writer.Write(value);
                return this;
            }

            public IBufferWriter WriteUShort(ushort value)
            {
                this._writer.Write(value);
                return this;
            }
        }

        public class MemoryBufferReader:IBufferReader
        {

            private readonly BinaryReader  _reader;
            public MemoryBufferReader(MemoryStream stream)
            {
                stream.Position = 0;
                this._reader = new BinaryReader(stream);
            }

            public int ReadInt()
            {
                return this._reader.ReadInt32();
            }

            public byte ReadByte()
            {
                return this._reader.ReadByte();
            }

            public long ReadLong()
            {
                 return _reader.ReadInt64();
            }

            public double ReadDouble()
            {
                return this._reader.ReadDouble();
            }

            public void ReadBytes(byte[] dist)
            {
               var temp  = this._reader.ReadBytes(dist.Length);
               Buffer.BlockCopy(temp,0,dist,0,temp.Length);
            }

            public char ReadChar()
            {
                return this._reader.ReadChar();
            }

            public short ReadShort()
            {
                return this._reader.ReadInt16();
            }

            public ushort ReadUShort()
            {
                return this._reader.ReadUInt16();
            }

            public uint ReadUInt()
            {
                return this._reader.ReadUInt32();
            }

            public int ReadableBytes => (int) (this._reader.BaseStream.Length - this._reader.BaseStream.Position);
        }
    }
}
