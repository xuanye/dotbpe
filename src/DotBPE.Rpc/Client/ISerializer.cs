namespace DotBPE.Rpc.Client
{
    public interface ISerializer
    {
        T Deserialize<T>(byte[] data);

        byte[] Serialize<T>(T item);
    }
}
