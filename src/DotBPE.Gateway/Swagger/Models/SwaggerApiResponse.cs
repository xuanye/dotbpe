using System.Runtime.Serialization;

namespace DotBPE.Gateway.Swagger
{

    [DataContract]
    public class SwaggerApiResponse
    {
        [DataMember(Name = "description")]
        public  string Description { get; set; }

        [DataMember(Name = "schema")]
        public SwaggerItemSchema Schema { get; set; }
    }
}
