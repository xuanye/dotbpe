using System.Collections.Generic;

namespace DotBPE.Rpc.Options
{
    public class RemoteServicesOption:List<ServiceOption>
    {

    }

    public class ServiceOption{
        public int ServiceId{get;set;}
        public string MessageIds{get;set;}

        public string RemoteAddress{get;set;}
    }
}