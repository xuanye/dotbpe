using DotBPE.Plugin.AspNetGateway;
using DotBPE.Protobuf;
using Microsoft.Extensions.DependencyInjection;
using Survey.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Survey.AspNetGateway
{
    public static class AspNetGatewayExtension
    {
        /// <summary>
        /// 扩展路由配置信息
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IServiceCollection AddRoutes(this IServiceCollection services)
        {
            services.Configure<HttpRouterOption>(opt =>
            {
                opt.CookieMode = CookieMode.Manual;

                if(opt.Items == null)
                {
                    opt.Items = new List<HttpRouterOptionItem>();
                }
                
                foreach(var item in HttpApiOptions.GetList())
                {
                    opt.Items.Add(new HttpRouterOptionItem()
                    {
                        ServiceId = item.ServiceId,
                        MessageId = item.MessageId,
                        Method = item.Method,
                        Path = item.Path
                    });
                }

            });
            return services;
        }
    }
}
