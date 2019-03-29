using System;

namespace DotBPE.Gateway
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RouterAttribute:Attribute
    {

        public RouterAttribute(string path,RestfulVerb acceptVerb= RestfulVerb.Any)
        {
            Path = path;
            AcceptVerb = acceptVerb;
        }


        public string Category { get; set; } = "default";
        public string Path { get; }

        public RestfulVerb AcceptVerb { get;  }

        public string PluginName { get; set; }


        private Type _PluginType;
        public Type PluginType
        {
            get
            {
                if (!string.IsNullOrEmpty(PluginName))
                {
                    if (_PluginType != null)
                    {
                        return _PluginType;
                    }
                    _PluginType =Type.GetType(PluginName);
                    return _PluginType;
                }

                return null;
            }
        }
    }
}
