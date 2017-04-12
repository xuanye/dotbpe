using System.Threading.Tasks;

namespace HelloRpc.Common
{
    public interface IGreeterClient
    {
        HelloResponse HelloPlus(HelloRequest request);
        Task<HelloResponse> HelloPlusAsnyc(HelloRequest request);
    }

    public interface IMathClient{
        addResponse Add(addRequest request);

        Task<addResponse> AddAsnyc(addRequest request);
    }

}