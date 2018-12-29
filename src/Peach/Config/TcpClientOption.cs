using System;
using System.Collections.Generic;
using System.Text;

namespace Peach.Config
{
    public class TcpClientOption
    {
        public bool TcpNodelay { get; set; } = true;
        public bool SoKeepalive { get; set; } = true;
        public int ConnectTimeout { get; set; } = 0;
    }
}
