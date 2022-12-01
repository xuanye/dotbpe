// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;

namespace DotBPE.Gateway
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpRouteAttribute : Attribute
    {

        public HttpRouteAttribute(string path, HttpVerb acceptVerb = HttpVerb.Any, string version = "1.0.0")
        {

            Path = path;
            AcceptVerb = acceptVerb;
            Version = version;
        }


        public string Category { get; set; } = "default";
        public string Path { get; }

        public string Version { get; } = "1.0.0";

        public HttpVerb AcceptVerb { get; }

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
