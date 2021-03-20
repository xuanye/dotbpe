using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace PipelineSample.Services
{

    [DataContract]
    public class VoidRes
    {
        [DataMember(Name = "returnMessage", Order = 1)]
        public string ReturnMessage { get; set; }
    }

    [DataContract]
    public class QueueTaskReq
    {
        [DataMember(Name = "delay", Order = 1)]
        public int Delay { get; set; }

        [DataMember(Name = "jobData", Order = 2)]
        public string JobData { get; set; }

        [DataMember(Name = "x-request-id", Order = 3)]
        public string XRequestId { get; set; }
    }
}
