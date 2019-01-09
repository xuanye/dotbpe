using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DotBPE.Gateway.Swagger
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSwagger(this IApplicationBuilder builder,string handlerPath="/swagger", Action<SwaggerConfig> configAction =null)
        {

            builder.ApplicationServices.GetRequiredService<IProtocolProcessor>();

            var swagger = builder.ApplicationServices.GetRequiredService<ISwaggerApiInfoProvider>();

            swagger.ScanApiInfo(configAction);

            builder.UseMiddleware<SwaggerMiddleware>(handlerPath);

            return builder;
        }
    }
}
