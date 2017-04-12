using System;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace DotBPE.Protocol.Amp
{
    public class AmpMessageRouter : IMessageRouter
    {
        public AmpMessageRouter(IConfiguration config){

        }
        public RouterPoint GetRouterPoint(AmpMessage message)
        {
            throw new NotImplementedException();
        }
    }


    public interface IMessageRouter{
        RouterPoint GetRouterPoint(AmpMessage message);
    }

    public class RouterPoint{
        public EndPoint RemoteAddress{get;set;}

        public RoutePointType RoutePointType {get;set;}
    }

    public enum RoutePointType{
        Local,
        Remote
    }
}