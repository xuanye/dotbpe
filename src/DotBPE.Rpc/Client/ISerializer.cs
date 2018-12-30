namespace DotBPE.Rpc.Client
{
    public interface ISerializer
    {
        T Decode<T>(byte[] data);

        byte[] Encode<T>(T item);
    }
}
