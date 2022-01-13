using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Gateway.Swagger
{
    public class SwaggerOptions
    {
        public SwaggerApiInfo ApiInfo { get; set; }


        public string Host { get; set; }
        public string BasePath { get; set; } = "/";


        public List<string> IngoreFields { get; } = new List<string> { "Identity", "ClientIp", "XRequestId", "Header" };

        public string RoutePath { get; set; } = "/v2/swagger.json";


    }
}
