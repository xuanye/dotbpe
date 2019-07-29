using System;
using System.IO;
using System.Reflection;

namespace Tomato.Gateway.SwaggerUI
{
    public class SwaggerUIOptions
    {

        public string SwaggerJsonPath { get; set; } = "/v2/swagger.json";
        /// <summary>
        /// Gets or sets a route prefix for accessing the swagger-ui
        /// </summary>
        public string RoutePrefix { get; set; } = "/swagger";

        /// <summary>
        /// Gets or sets a Stream function for retrieving the swagger-ui page
        /// </summary>
        public Func<Stream> IndexStream { get; set; } = () => typeof(SwaggerUIOptions).GetTypeInfo().Assembly
            .GetManifestResourceStream($"{typeof(SwaggerUIOptions).GetTypeInfo().Assembly.GetName().Name}.swagger_ui.index.html");

        /// <summary>
        /// Gets or sets a title for the swagger-ui page
        /// </summary>
        public string DocumentTitle { get; set; } = "Tomato HttpGateway";
    }
}
