using System.Collections.Generic;

namespace PiggyMetrics.Common
{
    public class RouterOption
    {
        public List<Router> Routers{get;set;}
    }

    public class Router{
        public string Path {get;set;}
        public string Method {get;set;}
        public int ServiceId{get;set;}
        public int MessageId{get;set;}

        public bool NeedAuth{get;set;}
    }
}
