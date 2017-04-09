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
            this.HostIP = string.IsNullOrEmpty(configuration["hostIp"]) ? "127.0.0.1" : configuration["hostIp"];
            this.HostPort = ParseInt(configuration, "hostPort", 6201);
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
