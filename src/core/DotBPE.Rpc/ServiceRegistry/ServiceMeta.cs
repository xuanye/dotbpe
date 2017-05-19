namespace DotBPE.Rpc.ServiceRegistry
{
    public class ServiceMeta
    {
        public string Id { get; set; }
        public int ServiceId {get;set;}
        public string ServiceName { get; set; }
        public string IPAddress { get; set; }
        public int Port { get; set; }
        public string FormatAddress => string.Format("{0}:{1}", IPAddress, Port);
        public string[] Tags {get;set;}
    }
}
