namespace DotBPE.Rpc.Server
{
    public interface IMethod
    {
        string ServiceName { get; }
        string MethodName { get; }
        int ServiceId { get; }
        int MethodId { get; }

        string FullName { get; }

        string Key { get; }
    }
}
