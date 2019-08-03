using System;
using System.Threading.Tasks;

namespace Tomato.Gateway.Swagger
{
    public interface ISwaggerApiInfoProvider
    {
        void ScanApiInfo(SwaggerConfig config);

        string GetSwaggerApiJson();
    }
}
