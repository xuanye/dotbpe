using System;
using System.Collections.Generic;
using System.Text;

namespace PiggyMetrics.Common.Consul.Service
{
    public partial class ServiceMeta
    {
        public string Id { get; set; }

        public int ServiceId => int.Parse(this.Id.Split('$')[1]);

        public string ServiceName { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public string Host => string.Format("{0}:{1}", Address, Port);

        public string[] Tags {get;set;}
    }
}
