namespace DotBPE.Rpc
{
    public class RpcResult
    {
        public int Code { get; set; }
    }

    public class RpcResult<T> : RpcResult
    {
        public T Data { get; set; }
    }

    public class RpcContentResult : RpcResult<string>
    {
        public string Message { get; set; }
    }
}
