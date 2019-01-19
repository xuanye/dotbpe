using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.Extensions.DependencyInjection;
using PiggyMetrics.Common;
using DotBPE.Rpc.Logging;

namespace PiggyMetrics.HttpApi
{
    public class AuthenticateMiddleware
    {

        static ILogger Logger = DotBPE.Rpc.Environment.Logger.ForType<AuthenticateMiddleware>();
        private readonly RequestDelegate _next;

        private readonly AuthenticateOption _option;

        public AuthenticateMiddleware(RequestDelegate next,  AuthenticateOption option = null)
        {
            _next = next;
            _option = option ?? new AuthenticateOption();

        }

        public async Task Invoke(HttpContext context)
        {
            bool matchLogin = context.Request.Path.Equals(_option.LoginPath,StringComparison.OrdinalIgnoreCase);
            bool matchCurrent = context.Request.Path.Equals(_option.CurrentPath,StringComparison.OrdinalIgnoreCase);
            bool matchLogout = context.Request.Path.Equals(_option.LogoutPath,StringComparison.OrdinalIgnoreCase);

            Logger.Debug("request path = {0}",context.Request.Path);
            if (matchLogin)
            {
                context.Response.ContentType = "application/json";
                LoginResult result = new LoginResult();


                var forward=  context.RequestServices.GetRequiredService<IForwardService>();
                var callresult = await forward.ForwardAysnc(context);
                if(callresult.Status != 0){
                    result.Status = -1;
                    result.Message = callresult.Message;
                    await context.Response.WriteAsync(result.ToString());
                    return ;
                }
                AuthRsp rsp = AuthRsp.Parser.ParseJson(callresult.Content);
                if(rsp ==null){
                    result.Status = -1;
                    result.Message = "Server Internal Error ,Wrong Encode!";
                    await context.Response.WriteAsync(result.ToString());
                    return ;
                }

                if (rsp.Status !=0){
                    result.Status = -1;
                    result.Message = rsp.Message;
                    await context.Response.WriteAsync(result.ToString());
                    return ;
                }
                const string Issuer = "https://PiggyMetrics.io";
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,rsp.Account, ClaimValueTypes.String, Issuer),

                };

                var userIdentity = new ClaimsIdentity("SuperSecureLogin");
                userIdentity.AddClaims(claims);
                var userPrincipal = new ClaimsPrincipal(userIdentity);

                await context.Authentication.SignInAsync(_option.AuthenticationScheme, userPrincipal,
                    new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                        IsPersistent = false,
                        AllowRefresh = false
                    });
                result.Status = 0;
                result.Account = rsp.Account;
                await context.Response.WriteAsync(result.ToString());
            }
            else if(matchCurrent){
                context.Response.ContentType = "application/json";
                LoginResult result = new LoginResult();
                if(!context.User.Identity.IsAuthenticated){
                    result.Status  = -1;
                    result.Message = "Need Authenticate";
                    context.Response.StatusCode = 501;
                }
                else{
                    result.Status  = 0;
                    result.Account = context.User.Identity.Name;
                }

                await context.Response.WriteAsync(result.ToString());
            }
            else if(matchLogout){
                context.Response.ContentType = "application/json";
                LoginResult result = new LoginResult();
                await context.Authentication.SignOutAsync(_option.AuthenticationScheme);
                await context.Response.WriteAsync(result.ToString());
            }
            else{
                await _next.Invoke(context);
            }

        }
    }

    public class AuthenticateOption{
        public string LoginPath {get;set;} = "/auth/login";
        public string LogoutPath {get;set;} = "/auth/logout";
        public string CurrentPath {get;set;} = "/auth/current";

        public string AuthenticationScheme{get;set;} = "PiggyMetrics";
    }

    public class LoginResult{
        public int Status{get;set;}
        public string Message{get;set;}
        public string Account{get;set;}

        public override string ToString(){
            string body = string.Format("\"status\":{0},\"message\":\"{1}\",\"account\":\"{2}\"",Status,Message,Account);

            return "{"+body+"}";
        }
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
