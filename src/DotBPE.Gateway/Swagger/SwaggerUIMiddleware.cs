﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DotBPE.Gateway.Swagger
{
    public class SwaggerUIMiddleware
    {
        private const string EmbeddedFileNamespace = "DotBPE.Gateway.Swagger.UI";

        private readonly SwaggerUIOptions _options;
        private readonly StaticFileMiddleware _staticFileMiddleware;

        public SwaggerUIMiddleware(
            RequestDelegate next,
            IWebHostEnvironment hostingEnv,
            ILoggerFactory loggerFactory,
            IOptions<SwaggerUIOptions> optionsAccessor)
            : this(next, hostingEnv, loggerFactory, optionsAccessor.Value)
        { }

        public SwaggerUIMiddleware(
            RequestDelegate next,
            IWebHostEnvironment hostingEnv,
            ILoggerFactory loggerFactory,
            SwaggerUIOptions options)
        {
            _options = options ?? new SwaggerUIOptions();
            _staticFileMiddleware = CreateStaticFileMiddleware(next, hostingEnv, loggerFactory, options);
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var httpMethod = httpContext.Request.Method;
            var path = httpContext.Request.Path.Value;

            string pattern = $"^{_options.RoutePrefix}/?$";
            // If the RoutePrefix is requested (with or without trailing slash), redirect to index URL
            if (httpMethod == "GET" && Regex.IsMatch(path, pattern))
            {
                // Use relative redirect to support proxy environments
                var relativeRedirectPath = path.EndsWith("/")
                    ? "index.html"
                    : $"{path.Split('/').Last()}/index.html";

                RespondWithRedirect(httpContext.Response, relativeRedirectPath);
                return;
            }

            if (httpMethod == "GET" && Regex.IsMatch(path, $"{_options.RoutePrefix}/?index.html"))
            {
                await RespondWithIndexHtml(httpContext.Response);
                return;
            }

            await _staticFileMiddleware.Invoke(httpContext);
        }

        private void RespondWithRedirect(HttpResponse response, string location)
        {
            response.StatusCode = 301;
            response.Headers["Location"] = location;
        }

        private async Task RespondWithIndexHtml(HttpResponse response)
        {
            response.StatusCode = 200;
            response.ContentType = "text/html";

            using (var stream = _options.IndexStream())
            {
                string body = "";
                using (var reader = new StreamReader(stream))
                {
                    body = reader.ReadToEnd();
                }
                // Inject arguments before writing to response
                var htmlBuilder = new StringBuilder(body);
                foreach (var entry in GetIndexArguments())
                {
                    htmlBuilder.Replace(entry.Key, entry.Value);
                }

                await response.WriteAsync(htmlBuilder.ToString(), Encoding.UTF8);
            }
        }

        private IDictionary<string, string> GetIndexArguments()
        {
            return new Dictionary<string, string>
            {
                { "%(DocumentTitle)", _options.DocumentTitle },
                { "%(SwaggerJsonPath)", _options.SwaggerJsonPath }
            };
        }



        private StaticFileMiddleware CreateStaticFileMiddleware(
            RequestDelegate next,
            IWebHostEnvironment hostingEnv,
            ILoggerFactory loggerFactory,
            SwaggerUIOptions options)
        {
            var staticFileOptions = new StaticFileOptions
            {
                RequestPath = string.IsNullOrEmpty(options.RoutePrefix) ? string.Empty : $"{options.RoutePrefix}",
                FileProvider = new EmbeddedFileProvider(typeof(SwaggerUIMiddleware).Assembly, EmbeddedFileNamespace)
            };

            return new StaticFileMiddleware(next, hostingEnv, Options.Create(staticFileOptions), loggerFactory);
        }
    }
}
