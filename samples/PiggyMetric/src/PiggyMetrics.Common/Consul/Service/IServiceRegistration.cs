
using System.Threading.Tasks;


namespace PiggyMetrics.Common.Consul.Service
{
    public interface IServiceRegistration
    {
        Task Register(ServiceMeta service);
    }

   
}
