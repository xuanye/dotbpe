using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace PiggyMetrics.HttpApi
{
    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder UseConsuleDiscovery(this IApplicationBuilder builder){
            builder.ApplicationServices.GetRequiredService<IBridgeRouter<AmpMessage>>(); // 获取示例，内部会自动初始化链接
            return builder;
        }
    }
}
