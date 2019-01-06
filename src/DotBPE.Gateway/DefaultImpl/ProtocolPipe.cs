using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DotBPE.Gateway
{
    public class ProtocolPipe:IPipeline
    {


        public Task<bool> Invoke(HttpContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
