
namespace DotBPE.Rpc.Codes
{
    public abstract class InvokeMessage:IMessage
    {
      

        public string UniqueId { get; set; }

        public InvokeMessageType InvokeMessageType { get; set; }

        public abstract int Length { get; }

    }


    public enum InvokeMessageType
    {
        Request = 1,
        Response = 2,
        Notify = 3
    }
}
