using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DotBPE.Gateway.Swagger.Generator
{
    [DataContract]
    public class SwaggerInfo
    {
        [DataMember(Name = "swagger")]
        public string Swagger { get; set; } = "2.0";

        [DataMember(Name = "info")]
        public SwaggerApiInfo Info { get; set; }

        [DataMember(Name = "schemes")]
        public string[] Schemes { get; set; } = {"http"};

        [DataMember(Name = "tags")]
        public List<SwaggerTag> Tags { get; set; }

        [DataMember(Name = "host")]
        public string Host { get; set; }

        [DataMember(Name = "basePath")]
        public string BasePath { get; set; }

        [DataMember(Name = "paths")]
        public Dictionary<string,Dictionary<string,SwaggerMethod>> Paths { get; set; }


        [DataMember(Name = "definitions")]
        public Dictionary<string,SwaggerDefinition> Definitions { get; set; }


    }


    [DataContract]
    public class SwaggerTag
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }
    }




}
