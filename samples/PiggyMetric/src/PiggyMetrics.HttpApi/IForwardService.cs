using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PiggyMetrics.Common;

namespace PiggyMetrics.HttpApi
{
    public interface IForwardService
    {
        Task<CallResult> ForwardAysnc(HttpContext context);
    }
}
