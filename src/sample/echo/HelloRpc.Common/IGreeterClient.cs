using System.Threading.Tasks;

namespace HelloRpc.Common
{
    public interface IGreeterClient
    {
        HelloReply SayHello(HelloRequest request);
        Task<HelloReply> SayHelloAsnyc(HelloRequest request);
    }
}