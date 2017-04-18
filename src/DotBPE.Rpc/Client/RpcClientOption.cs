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

            var multiplexCount = ParseInt(configuration,"multiplexCount",1);
            if(multiplexCount<1){
                multiplexCount = 1;
            }
            MultiplexCount = multiplexCount ;
        }

        /// <summary>
        ///  客户端多链接数量，默认是1，即不使用多工，使用单链接
        /// </summary>
        /// <returns></returns>
        public int MultiplexCount {
            get;private set;
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
