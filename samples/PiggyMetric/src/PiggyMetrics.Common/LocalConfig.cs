using System;
using Microsoft.Extensions.Configuration;

namespace PiggyMetrics.Common
{
    public class LocalConfig
    {
        public string AppName { get; set; }
        public string ConsulServer { get; set; }
        public string HostAddress { get; set; }

        public static LocalConfig Load()
        {
            var localConfiguration = new ConfigurationBuilder()
                .AddJsonFile("dotbpe.config.json").Build();
            LocalConfig config = new LocalConfig
            {
                ConsulServer = localConfiguration["consul:server"],
                AppName = localConfiguration["appName"] ?? "PiggyMetrics",
                HostAddress = localConfiguration["hostAddress"],
                RequireService =  localConfiguration["requireService"]
            };
            return config;
        }

        public string RequireService{get;set;}
        public string Address
        {
            get
            {
                var arr = this.HostAddress.Split(':');
                if (arr.Length != 2)
                {
                    throw new InvalidCastException($"HostAddress 格式不正确 {HostAddress}");
                }
                return arr[0];
            }
        }
        public int Port
        {
            get
            {
                var arr = this.HostAddress.Split(':');
                if (arr.Length != 2)
                {
                    throw new InvalidCastException($"HostAddress 格式不正确 {HostAddress}");
                }
                return int.Parse(arr[1]);
            }
        }
    }
}
