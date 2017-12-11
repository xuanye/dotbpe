using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Plugin.Gateway
{
    /// <summary>
    /// WebApi路由配置类
    /// </summary>
    public class WebApiRouterOption
    {
        public List<WebApiRouterOptionItem> Items { get; set; }
    }

    /// <summary>
    /// 路由配置类明细，用于配置某个请求路径对应某个服务的某个消息
    /// </summary>
    public class WebApiRouterOptionItem
    {
        public string Path { get; set; }
        public string Method { get; set; }
        public int ServiceId { get; set; }
        public int MessageId { get; set; }
        public bool NeedAuth { get; set; }
    }
}
