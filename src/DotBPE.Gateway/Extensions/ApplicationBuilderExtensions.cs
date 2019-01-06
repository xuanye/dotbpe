using Microsoft.AspNetCore.Builder;

namespace DotBPE.Gateway
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseGateway(this IApplicationBuilder builder)
        {

            builder.UseMiddleware<GatewayMiddleware>();

            return builder;
        }
    }
}
