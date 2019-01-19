using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DotBPE.Gateway
{
    public interface IProtocolProcessor
    {
        Task<bool> Invoke(HttpContext context);
    }
}
