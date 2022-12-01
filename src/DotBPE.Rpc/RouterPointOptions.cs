// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc
{
    public class RouterPointOptions
    {
        public List<GroupIdentifierOption> Categories { get; set; }
        public List<ServiceIdentifierOption> Services { get; set; }
        public List<MessageIdentifierOption> Messages { get; set; }

    }

    public class GroupIdentifierOption
    {
        public string GroupName { get; set; }
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
