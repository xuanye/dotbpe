using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Plugin.AspNetGateway
{
    /// <summary>
    /// Http路由配置类
    /// </summary>
    public class HttpRouterOption
    {
        public CookieMode CookieMode { get; set; }
        public List<HttpRouterOptionItem> Items { get; set; }
    }

    /// <summary>
    /// 路由配置类明细，用于配置某个请求路径对应某个服务的某个消息
    /// </summary>
    public class HttpRouterOptionItem
    {
        public string Path { get; set; }
        public string Method { get; set; }
        public int ServiceId { get; set; }
        public int MessageId { get; set; }
    }

    public enum CookieMode
    {
        None = 0,
        Auto = 1,
        Manual = 2
    }
}
