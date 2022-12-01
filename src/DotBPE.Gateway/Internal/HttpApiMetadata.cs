// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DotBPE.Gateway.Internal
{
    internal class HttpApiMetadata
    {
        public HttpApiMetadata(MethodInfo handerMethod, HttpApiOptions httpApiOptions, Type inputType, Type outputType)
        {
            HanderMethod = handerMethod;
            HttpApiOptions = httpApiOptions;
            InputType = inputType;
            OutputType = outputType;
        }

        public Type HanderServiceType => HanderMethod?.DeclaringType;

        public Type InputType { get; }
        public Type OutputType { get; }

        public MethodInfo HanderMethod { get; }
        public HttpApiOptions HttpApiOptions { get; }
    }
}
