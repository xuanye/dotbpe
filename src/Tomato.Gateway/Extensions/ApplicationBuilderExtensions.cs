using Tomato.Rpc.Internal;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Tomato.Gateway
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseGateway(this IApplicationBuilder builder)
        {
            var loggerFactory = builder.ApplicationServices.GetService<ILoggerFactory>();
            Environment.SetServiceProvider(builder.ApplicationServices);
            Environment.SetLoggerFactory(loggerFactory);

            builder.UseMiddleware<GatewayMiddleware>();

            return builder;
        }
    }
}
