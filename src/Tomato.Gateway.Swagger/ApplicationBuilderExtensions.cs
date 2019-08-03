using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Tomato.Gateway.Swagger
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSwagger(this IApplicationBuilder builder,Action<SwaggerConfig> configAction =null)
        {

            builder.ApplicationServices.GetRequiredService<IProtocolProcessor>();

            var swagger = builder.ApplicationServices.GetRequiredService<ISwaggerApiInfoProvider>();
            SwaggerConfig config = new SwaggerConfig();
            configAction?.Invoke(config);

            swagger.ScanApiInfo(config);

            builder.UseMiddleware<SwaggerMiddleware>(config.RoutePath);

            return builder;
        }
    }
}
