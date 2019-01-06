using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DotBPE.Gateway
{
    public interface IPipeline
    {
        Task<bool> Invoke(HttpContext context);
    }
}
