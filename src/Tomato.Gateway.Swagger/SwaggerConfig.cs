using System.Collections.Generic;
using Tomato.Gateway.Swagger.Generator;

namespace Tomato.Gateway.Swagger
{
    public class SwaggerConfig
    {

        public SwaggerApiInfo ApiInfo { get; set; }
        public string Host { get; set; } = "localhost";
        public string BasePath { get; set; } = "/";
        public List<string> XmlComments { get;  } = new List<string>();


        public List<string> IngoreFields { get; } = new List<string>{"Identity","ClientIp","XRequestId"};

        public string RoutePath { get; set; } = "/v2/swagger.json";

        public void IncludeXmlComments(string xmlPath)
        {
            XmlComments.Add(xmlPath);
        }
    }


}
