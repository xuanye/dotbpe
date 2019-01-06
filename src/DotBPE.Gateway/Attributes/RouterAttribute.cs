using System;

namespace DotBPE.Gateway
{
    public class RouterAttribute:Attribute
    {

        public RouterAttribute(string pattern,RestfulVerb acceptVerb= RestfulVerb.Any)
        {
            this.RoutePattern = pattern;
            this.AcceptVerb = acceptVerb;
        }

        public string RoutePattern { get; }

        public RestfulVerb AcceptVerb { get;  }
    }
}
