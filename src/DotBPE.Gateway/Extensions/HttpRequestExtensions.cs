// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System.Linq;


namespace Microsoft.AspNetCore.Http
{
    public static class HttpRequestExtensions
    {
        public static string GetClientIp(this HttpRequest request)
        {
            var ip = request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (string.IsNullOrEmpty(ip))
            {
                ip = request.Headers["X-Real-IP"].FirstOrDefault();
            }

            if (string.IsNullOrEmpty(ip))
            {
                var IPAddress = request.HttpContext.Connection.RemoteIpAddress;

                ip = IPAddress.IsIPv4MappedToIPv6 ? IPAddress.MapToIPv4().ToString() : IPAddress.ToString();
            }
            return ip;
        }
    }

}
