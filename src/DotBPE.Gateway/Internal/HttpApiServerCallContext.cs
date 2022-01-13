using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DotBPE.Gateway.Internal
{
    internal class HttpApiServerCallContext : ServerCallContext
    {
        private readonly HttpContext _httpContext;
        private readonly string _methodFullName;
        private string _peer;
        //private Metadata _requestHeaders;

        public HttpApiServerCallContext(HttpContext httpContext, string methodFullName)
        {
            _httpContext = httpContext;
            _methodFullName = methodFullName;
            // Add the HttpContext to UserState so GetHttpContext() continues to work
            _httpContext.Items["__HttpContext"] = httpContext;
        }

        protected override CancellationToken CancellationTokenCore => _httpContext.RequestAborted;


        protected override string MethodCore => _methodFullName;


        protected override string HostCore => _httpContext.Request.Host.Value;


        protected override DateTime DeadlineCore { get; }

        protected override IDictionary<object, object> UserStateCore=> _httpContext.Items;

        protected override string PeerCore
        {
            get
            {
               
                if (_peer == null)
                {
                    var connection = _httpContext.Connection;
                    if (connection.RemoteIpAddress != null)
                    {
                        switch (connection.RemoteIpAddress.AddressFamily)
                        {
                            case AddressFamily.InterNetwork:
                                _peer = "ipv4:" + connection.RemoteIpAddress + ":" + connection.RemotePort;
                                break;
                            case AddressFamily.InterNetworkV6:
                                _peer = "ipv6:[" + connection.RemoteIpAddress + "]:" + connection.RemotePort;
                                break;
                            default:
                                // TODO(JamesNK) - Test what should be output when used with UDS and named pipes
                                _peer = "unknown:" + connection.RemoteIpAddress + ":" + connection.RemotePort;
                                break;
                        }
                    }
                }

                return _peer;
            }
        }
    }
}
