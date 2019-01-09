using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DotBPE.Gateway.Swagger
{
    public class SwaggerMiddleware
    {
        private readonly string _handlerPath;
        private readonly ISwaggerApiInfoProvider _provider;
        private readonly RequestDelegate _next;


        public SwaggerMiddleware(string handlerPath,ISwaggerApiInfoProvider provider,RequestDelegate next)
        {
            this._handlerPath = handlerPath;
            this._provider = provider;
            this._next = next;

        }


        public async Task Invoke(HttpContext context)
        {
            if (!this._handlerPath.Equals(context.Request.Path, StringComparison.CurrentCultureIgnoreCase))
            {
                await this._next(context);
            }

            string json = this._provider.GetSwaggerApiJson();
            if (string.IsNullOrEmpty(json))
            {
                context.Response.StatusCode = (int) HttpStatusCode.NotFound;
                context.Response.ContentType = "plain/txt";
                await context.Response.WriteAsync("swagger is not ready");
            }
            else
            {   context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(json);
            }
            //TODO:Use Handle

        }

    }
}
