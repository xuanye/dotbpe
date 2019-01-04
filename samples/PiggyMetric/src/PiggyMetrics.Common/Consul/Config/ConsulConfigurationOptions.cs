using System;
using Consul;

namespace PiggyMetrics.Common
{
    public class ConsulConfigurationOptions
    {
        public Uri ConsulAddress{get;set;}

        /// <summary>
        ///  Consul中配置的根信息
        /// </summary>
        /// <returns></returns>
        public string Key{get;set;}

        /// <summary>
        /// 是否可选
        /// </summary>
        /// <returns></returns>
        public bool Optional {get;set;}

        public bool ReloadOnChange{get;set;}

        public Action<Exception,ExceptionCaughtEventArgs> ExceptionCaught;

        public int  CheckInterval {get;set;} = 30000;

        public Action<ConsulClientConfiguration> ConsulClientConfigOverride;
    }

    public class ExceptionCaughtEventArgs{
        public bool Ingore{get;set;}
    }
}
