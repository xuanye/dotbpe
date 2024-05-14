// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Gateway.Swagger;
using DotBPE.Rpc.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSwagger(this IApplicationBuilder builder)
        {
            var optionsAccessor = builder.ApplicationServices.GetRequiredService<IOptions<SwaggerOptions>>();
            var options = optionsAccessor.Value ?? new SwaggerOptions();
            //var swaggerProvider = builder.ApplicationServices.GetRequiredService<ISwaggerApiInfoProvider>();
            //swaggerProvider.ExtractApiInfo();

            builder.UseMiddleware<SwaggerMiddleware>(options);

            return builder;
        }

        public static IApplicationBuilder UseSwaggerUI(this IApplicationBuilder app, Action<SwaggerUIOptions> setupAction = null)
        {
            if (setupAction == null)
            {
                // Don't pass options so it can be configured/injected via DI container instead
                app.UseMiddleware<SwaggerUIMiddleware>();
            }
            else
            {
                // Configure an options instance here and pass directly to the middleware
                var options = new SwaggerUIOptions();
                setupAction.Invoke(options);

                app.UseMiddleware<SwaggerUIMiddleware>(options);
            }

            return app;
        }

        /// <summary>
        /// When the Http service is started at the same time as the Rpc service,
        /// We need to register all the actor methods with the Rpc service.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder RunRpcServer(this IApplicationBuilder builder)
        {
            var actorBuilder = builder.ApplicationServices.GetRequiredService<IServiceActorBuilder>();
            actorBuilder.Build();

            return builder;
        }
    }
}