namespace DotBPE.Rpc.Codes {
    public interface IBufferReader {
        int ReadInt ();

        byte ReadByte ();

        long ReadLong ();

        double ReadDouble ();

        void ReadBytes (byte[] dist);

        char ReadChar ();

        short ReadShort ();

        ushort ReadUShort ();

        uint ReadUInt ();

        int ReadableBytes { get; }
    }
}