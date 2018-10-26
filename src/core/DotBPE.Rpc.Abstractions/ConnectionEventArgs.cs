using System.Net;

namespace DotBPE.Rpc
{
    public class ConnectionEventArgs
    {
        public EndPoint LocalPoint { get; set; }

        public EndPoint RemotePoint { get; set; }

        public string ChannelId { get; set; }
    }

}
