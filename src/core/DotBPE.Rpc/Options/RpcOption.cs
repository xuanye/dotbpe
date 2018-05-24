namespace DotBPE.Rpc.Options
{
    public class RpcOption { }

    public class RpcClientOption : RpcOption
    {
        public string DefaultServerAddress { get; set; }
        public int MultiplexCount { get; set; }
    }
}
