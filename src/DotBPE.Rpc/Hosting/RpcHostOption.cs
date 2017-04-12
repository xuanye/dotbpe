using System;
using Microsoft.Extensions.Configuration;

namespace DotBPE.Rpc.Hosting
{
    public class RpcHostOption
    {
        public RpcHostOption()
        { }

        public RpcHostOption(IConfiguration configuration)
        {
            //初始化一些属性咯
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            this.ApplicationName = configuration["appName"];

            string localAddress = configuration["hostAddress"];
            if (string.IsNullOrEmpty(localAddress))
            {
                localAddress = "127.0.0.1:6201";
            }
            string[] arr_Address = localAddress.Split(':');
            if(arr_Address.Length != 2)
            {
                throw new ArgumentException("地址配置错误："+ localAddress);
            }
            this.HostIP = arr_Address[0];
            this.HostPort = int.Parse(arr_Address[1]);
        }

        public string ApplicationName { get; set; }

        public string HostIP { get; set; }

        public int HostPort { get; set; }

        private static int ParseInt(IConfiguration configuration, string key,int defaultValue)
        {
            string value = configuration[key];

            if (int.TryParse(value, out var ret))
            {
                return ret;
            }
            return defaultValue;
        }
        private static bool ParseBool(IConfiguration configuration, string key)
        {
            return string.Equals("true", configuration[key], StringComparison.OrdinalIgnoreCase)
                   || string.Equals("1", configuration[key], StringComparison.OrdinalIgnoreCase);
        }
    }
}
