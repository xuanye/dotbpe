using System.Net;

namespace DotBPE.Extra
{
    public class ConsulOptions
    {
        public static ConsulOptions Default = new ConsulOptions
        {
            HttpEndpoint="http://127.0.0.1:8500",
            DnsEndpoint = new DnsEndpoint
            {
                Address =  "127.0.0.1",
                Port =  8600
            }
        };

        public string HttpEndpoint { get; set; } = "http://127.0.0.1:8500";

        public DnsEndpoint DnsEndpoint { get; set; }}

    public class DnsEndpoint
    {

        public string Address { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 8600;

        public IPEndPoint ToIPEndPoint()
        {
            return new IPEndPoint(IPAddress.Parse(Address), Port);
        }
    }
}
