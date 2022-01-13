using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Gateway.Swagger
{
    public interface ISwaggerProvider
    {
        SwaggerInfo GetSwaggerInfo();
    }
}
