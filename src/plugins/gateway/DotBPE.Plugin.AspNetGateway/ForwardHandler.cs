using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace DotBPE.Plugin.AspNetGateway
{
    public class ForwardHandler
    {
        public static async Task Process(IForwardService service, HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            var result = await service.ForwardAysnc(context);
            if (result.Code == 0 || result.Code == 200)
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result.Data);
            }
            else if (result.Code == 404)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync(result.Message ?? "Service not found!");
            }
            else
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync(result.Message ?? "Server Internal Error!");
            }
        }
    }
}
