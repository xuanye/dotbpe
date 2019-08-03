using System.Threading.Tasks;

namespace Tomato.Rpc.ServiceDiscovery
{
    public interface IServiceRegister
    {
        Task RegisterAllServices();
        Task DeregisterAllServices();
    }
}
