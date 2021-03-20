using System.Runtime.Serialization;

namespace DotBPE.Gateway.Swagger.Generator
{
    [DataContract]
    public class SwaggerApiParameters
    {
        [DataMember(Name = "in")]
        public string In { get; set; } = "query";//body formData
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "type")]
        public string Type { get; set; }
        [DataMember(Name = "description")]
        public  string Description { get; set; }

        [DataMember(Name = "schema")]
        public SwaggerItemSchema Schema { get; set; }
    }

}
