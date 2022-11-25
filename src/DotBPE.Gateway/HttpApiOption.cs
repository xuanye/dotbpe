// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;

namespace DotBPE.Gateway
{
    public class HttpApiOptions
    {

        public string Category { get; set; } = "default";
        public string Pattern { get; set; }

        public string Version { get; set; } = "1.0.0";

        public HttpVerb AcceptVerb { get; set; }

        public string PluginName { get; set; }


        private Type _pluginType;
        public Type PluginType
        {
            get
            {
                if (!string.IsNullOrEmpty(PluginName))
                {
                    if (_pluginType != null)
                    {
                        return _pluginType;
                    }
                    _pluginType = Type.GetType(PluginName);
                    return _pluginType;
                }

                return null;
            }
        }

    }
}
