using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DotBPE.Gateway
{
    public class HttpApiOptions
    {    

        public string Category { get; set; } = "default";
        public string Pattern { get; set; }

        public string Version { get; set; } = "1.0.0";

        public RestfulVerb AcceptVerb { get; set; }

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
                    _PluginType = Type.GetType(PluginName);
                    return _PluginType;
                }

                return null;
            }
        }     
       
    }
}
