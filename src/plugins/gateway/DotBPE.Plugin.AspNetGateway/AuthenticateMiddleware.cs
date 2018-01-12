using DotBPE.Rpc.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace DotBPE.Plugin.AspNetGateway
{
    /// <summary>
    /// 基于Cookie的用户认证插件
    /// </summary>
    public class AuthenticateMiddleware
    {
        private static ILogger Logger = DotBPE.Rpc.Environment.Logger.ForType<AuthenticateMiddleware>();
        private readonly RequestDelegate _next;

        private readonly AuthenticateOption _option;

        public AuthenticateMiddleware(RequestDelegate next, AuthenticateOption option = null)
        {
            _next = next;
            _option = option ?? new AuthenticateOption();
        }

        public async Task Invoke(HttpContext context)
        {
            bool matchLogin = context.Request.Path.Equals(_option.LoginPath, StringComparison.OrdinalIgnoreCase);
            bool matchLogout = context.Request.Path.Equals(_option.LogoutPath, StringComparison.OrdinalIgnoreCase);

            Logger.Debug("request path = {0}", context.Request.Path);
            if (matchLogin)
            {
                var auth = context.RequestServices.GetRequiredService<IAuthService>();
                await auth.LoginAsync(context, this._option);
            }
            else if (matchLogout)
            {
                var auth = context.RequestServices.GetRequiredService<IAuthService>();
                await auth.LogoutAsync(context);
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }

    public class AuthenticateOption
    {
        public string LoginPath { get; set; } = "/auth/login";
        public string LogoutPath { get; set; } = "/auth/logout";
        public string AuthenticationScheme { get; set; } = "DotBPE_WEBAPI";
    }

    public static class AuthenticateMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthenticate(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticateMiddleware>();
        }

        public static IApplicationBuilder UseAuthenticate(this IApplicationBuilder builder, AuthenticateOption option)
        {
            return builder.UseMiddleware<AuthenticateMiddleware>(option);
        }
    }
}
