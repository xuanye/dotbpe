using System.IO;
using Peach.Buffer;

namespace Tomato.Rpc.Tests
{
    public class MemoryBufferWriter : IBufferWriter
    {
        private readonly BinaryWriter _writer;

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
}
