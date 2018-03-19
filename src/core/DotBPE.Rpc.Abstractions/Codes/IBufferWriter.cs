namespace DotBPE.Rpc.Codes {
    public interface IBufferWriter {
        IBufferWriter WriteInt (int value);

        IBufferWriter WriteByte (byte value);

        IBufferWriter WriteLong (long value);

        IBufferWriter WriteDouble (double value);

        IBufferWriter WriteBytes (byte[] value);

        IBufferWriter WriteChar (char value);

        IBufferWriter WriteShort (short value);

        IBufferWriter WriteUInt (uint value);

        IBufferWriter WriteUShort (ushort value);
    }
}