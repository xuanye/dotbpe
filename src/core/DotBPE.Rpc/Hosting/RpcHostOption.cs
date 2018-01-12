using Microsoft.Extensions.Configuration;
using System;

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
            this.ApplicationName = configuration[HostDefaultKey.APPNAME_KEY];
            this.EnvironmentName = configuration[HostDefaultKey.EnvironmentName_KEY];
            if (string.IsNullOrEmpty(EnvironmentName))
            {
                this.EnvironmentName = Hosting.EnvironmentName.Production;
            }
            if (this.EnvironmentName != Hosting.EnvironmentName.Production
                && this.EnvironmentName != Hosting.EnvironmentName.Development
                && this.EnvironmentName != Hosting.EnvironmentName.Staging
             )
            {
                throw new ArgumentException(string.Format("environment config error:" + this.EnvironmentName + " should be one of {0},{1},{2}"
                , Hosting.EnvironmentName.Development,
                Hosting.EnvironmentName.Production,
                Hosting.EnvironmentName.Staging));
            }

            string localAddress = configuration[HostDefaultKey.HOSTADDRESS_KEY];
            if (string.IsNullOrEmpty(localAddress))
            {
                localAddress = "0.0.0.0:6201";
            }
            string[] arr_Address = localAddress.Split(':');
            if (arr_Address.Length != 2)
            {
                throw new ArgumentException("server address error" + localAddress);
            }
            this.HostIP = arr_Address[0];
            this.HostPort = int.Parse(arr_Address[1]);

            this.StartupType = configuration[HostDefaultKey.STARTUPTYPE_KEY];
        }

        public string ApplicationName { get; set; }

        public string HostIP { get; set; }

        public int HostPort { get; set; }

        public string EnvironmentName { get; set; }

        public string StartupType { get; set; }

        private static int ParseInt(IConfiguration configuration, string key, int defaultValue)
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
