using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DotBPE.Gateway
{
    public class RpcHttpMetadata
    {

      
        public RpcHttpMetadata(MethodInfo handerMethod,  HttpApiOptions httpApiOptions,Type inputType,Type outputType)
        {
            HanderMethod = handerMethod;          
            HttpApiOptions = httpApiOptions;
            InputType = inputType;
            OutputType = outputType;
        }

        public Type HanderServiceType
        {
            get
            {
                return this.HanderMethod?.DeclaringType;
            }
        }
        public Type InputType { get;}

        public Type OutputType { get;}
        public MethodInfo HanderMethod { get; }
              
      
        public HttpApiOptions HttpApiOptions { get; }
    }
}
