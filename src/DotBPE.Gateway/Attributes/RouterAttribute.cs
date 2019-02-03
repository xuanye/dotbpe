using System;

namespace DotBPE.Gateway
{
    public class RouterAttribute:Attribute
    {

        public RouterAttribute(string path,RestfulVerb acceptVerb= RestfulVerb.Any)
        {
            this.Path = path;
            this.AcceptVerb = acceptVerb;
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
                        return this._PluginType;
                    }
                    this._PluginType =Type.GetType(this.PluginName);
                    return this._PluginType;
                }

                return null;
            }
        }
    }
}
