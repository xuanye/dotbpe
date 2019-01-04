using System;
using System.IO;
using Peach.Buffer;

namespace DotBPE.Rpc.Tests
{
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
