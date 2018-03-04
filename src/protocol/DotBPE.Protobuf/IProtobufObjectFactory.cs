using Google.Protobuf;

namespace DotBPE.Protobuf
{
    public interface IProtobufObjectFactory
    {
        IMessage GetRequestTemplate(int serviceId, int messageId);

        IMessage GetResponseTemplate(int serviceId, int messageId);
    }
}
