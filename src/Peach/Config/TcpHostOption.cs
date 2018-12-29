using System;
using System.Collections.Generic;
using System.Text;

namespace Peach.Config
{
    public class TcpHostOption
    {
        public AddressBindType BindType { get; set; } = AddressBindType.LocalAddress;

        public int Port { get; set; } = 5566;

        public int SoBacklog { get; set; } = 128;


        public int QuietPeriod { get; set; } = 3;

        public int ShutdownTimeout { get; set; } = 3;

        public bool UseLibuv { get; set; } = true;
    }

    public enum AddressBindType
    {
        All,
        LocalAddress
    }
}
