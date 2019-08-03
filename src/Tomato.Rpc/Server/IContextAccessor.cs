namespace Tomato.Rpc.Server
{
    public interface IContextAccessor
    {
        ICallContext CallContext { get; set; }
    }
}
