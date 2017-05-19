
namespace DotBPE.Rpc.Codes
{

    public abstract class InvokeMessage : IMessage
    {
        public InvokeMessageType InvokeMessageType { get; set; }

        public abstract int Length { get; }


        /// <summary>
        /// 服务识别符,定位到某个服务
        /// </summary>
        public abstract string ServiceIdentifier { get; }
        /// <summary>
        /// 方法识别符，定位到某个方法
        /// </summary>
        public abstract string MethodIdentifier { get; }
    }

    public class InvokeMessageTypeParser
    {
        public static InvokeMessageType Parse(byte byteValue)
        {
            InvokeMessageType type;
            switch (byteValue)
            {
                case 1:
                    type = InvokeMessageType.Request;
                    break;
                case 2:
                    type = InvokeMessageType.Response;
                    break;
                case 3:
                    type = InvokeMessageType.Notify;
                    break;
                case 4:
                    type = InvokeMessageType.NotFound;
                    break;
                default:
                    type = InvokeMessageType.ERROR;
                    break;
            }
            return type;
        }
    }

    public enum InvokeMessageType : byte
    {
        Request = 1,
        Response = 2,
        Notify = 3,
        NotFound = 4,
        ERROR = 5
    }
}
