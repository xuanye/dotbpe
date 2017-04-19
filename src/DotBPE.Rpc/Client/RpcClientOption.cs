using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace DotBPE.Rpc
{
    public class RpcClientOption
    {
        public RpcClientOption()
        {
            
        }

        public RpcClientOption(IConfiguration configuration)
        {
            //初始化一些属性咯
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
          
        }
        private static int ParseInt(IConfiguration configuration, string key, int defaultValue)
        {
            string value = configuration[key];

            if (int.TryParse(value, out var ret))
            {
                return ret;
            }
            return defaultValue;
        }
    }
}
