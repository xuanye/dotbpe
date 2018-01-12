using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace DotBPE.Plugin.AspNetGateway
{
    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder UseGateWay(this IApplicationBuilder builder)
        {          
            builder.Run(Proccess);
            return builder;
        }

        private static Task Proccess(HttpContext context)
        {
            var service = context.RequestServices.GetRequiredService<IForwardService>();
            return ForwardHandler.Process(service, context);
        }
    }
}
