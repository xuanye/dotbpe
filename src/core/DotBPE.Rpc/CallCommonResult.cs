namespace DotBPE.Rpc
{
    public class CallCommonResult
    {
        public int Status{get;set;}

        public string Message{get;set;}
    }

    public class CallCommonResult<T>:CallCommonResult
    {
        public T Data{get;set;}
    }
    public class CallContentResult:CallCommonResult<string>
    {
    }
}
