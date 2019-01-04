using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Config
{
    public class RouterPointOptions
    {
        public List<CategoryIdentifierOption> Categories { get; set; } = new List<CategoryIdentifierOption>();

        public List<ServiceIdentifierOption> Services { get; set; } = new List<ServiceIdentifierOption>();

        public List<MessageIdentifierOption> Messages { get; set; } = new List<MessageIdentifierOption>();

    }

    public class CategoryIdentifierOption
    {
        public string Category { get; set; }
        public string RemoteAddress { get; set; }
        public int Weight { get; set; } = 1;
    }

    public class ServiceIdentifierOption
    {
        public int ServiceId { get; set; }
        public string RemoteAddress { get; set; }
        public int Weight { get; set; } = 1;
    }

    public class MessageIdentifierOption
    {
        public int ServiceId { get; set; }
        public int MessageId { get; set; }
        public string RemoteAddress { get; set; }
        public int Weight { get; set; } = 1;
    }
}
