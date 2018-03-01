using System.Threading.Tasks;

namespace DotBPE.Rpc.ServiceRegistry
{
    public interface IServiceRegistrationProvider
    {
        Task RegisterAsync(ServiceMeta service);
    }
}
