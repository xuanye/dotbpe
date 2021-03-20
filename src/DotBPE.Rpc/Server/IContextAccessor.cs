namespace DotBPE.Rpc.Server
{
    public interface IContextAccessor
    {
        ICallContext CallContext { get; set; }
    }
}
