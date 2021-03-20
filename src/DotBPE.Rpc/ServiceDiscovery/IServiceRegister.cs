using System.Threading.Tasks;

namespace DotBPE.Rpc.ServiceDiscovery
{
    public interface IServiceRegister
    {
        Task RegisterAllServices();
        Task DeregisterAllServices();
    }
}
