using System;

namespace DotBPE.Gateway
{
    public class RouterAttribute:Attribute
    {

        public RouterAttribute(string path,RestfulVerb acceptVerb= RestfulVerb.Any)
        {
            this.Path = Path;
            this.AcceptVerb = acceptVerb;
        }


        public string Category { get; set; }
        public string Path { get; }

        public RestfulVerb AcceptVerb { get;  }
        public  Type PluginType { get; set; }

    }
}
