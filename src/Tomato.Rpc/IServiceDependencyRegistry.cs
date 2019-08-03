using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tomato.Rpc
{
    /// <summary>
    /// 服务注册依赖的统一接口
    /// </summary>
    public interface IServiceDependencyRegistry
    {
        IServiceCollection AddServiceDependency(IConfiguration configuration, IServiceCollection services);
    }
}
