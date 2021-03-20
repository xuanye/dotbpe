using System.Runtime.Serialization;

namespace DotBPE.Gateway.Swagger.Generator
{
    [DataContract]
    public class SwaggerPropertyDefinition
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "format")]
        public string Format { get; set; }

        [DataMember(Name = "$ref")]
        public string Ref{get; set; }

        [DataMember(Name = "example")]
        public string Example { get; set; }

        [DataMember(Name = "items")]
        public SwaggerItemSchema Items { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }
    }
}
