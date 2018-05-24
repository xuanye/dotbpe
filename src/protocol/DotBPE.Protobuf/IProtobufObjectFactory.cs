using Google.Protobuf;

namespace DotBPE.Protobuf
{
    /// <summary>
    /// 协议对象工厂用于根据消息ID获取生成的消息定义
    /// 需要在特定的项目中实现，可使用代码生成工具实现
    /// </summary>
    public interface IProtobufObjectFactory
    {
        IMessage GetRequestTemplate(int serviceId, int messageId);

        IMessage GetResponseTemplate(int serviceId, int messageId);
    }
}
