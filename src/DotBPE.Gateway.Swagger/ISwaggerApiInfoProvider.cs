using System;
using System.Threading.Tasks;

namespace DotBPE.Gateway.Swagger
{
    public interface ISwaggerApiInfoProvider
    {
        void ScanApiInfo(SwaggerConfig config);

        string GetSwaggerApiJson();
    }
}
