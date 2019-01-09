using System.Collections.Generic;
using DotBPE.Gateway.Swagger.Generator;

namespace DotBPE.Gateway.Swagger
{
    public class SwaggerConfig
    {

        public SwaggerApiInfo ApiInfo { get; set; }
        public string Host { get; set; } = "localhost";
        public string BasePath { get; set; } = "/";
        public List<string> XmlComments { get;  } = new List<string>();

        public void IncludeXmlComments(string xmlPath)
        {
            XmlComments.Add(xmlPath);
        }
    }


}
