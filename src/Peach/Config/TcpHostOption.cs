using System;
using System.Collections.Generic;
using System.Text;

namespace Peach.Config
{
    public class TcpHostOption
    {
        public AddressBindType BindType { get; set; } = AddressBindType.InternalAddress;

        public int Port { get; set; } = 5566;

        public int SoBacklog { get; set; } = 128;


        public int QuietPeriod { get; set; } = 3;

        public int ShutdownTimeout { get; set; } = 3;

        public bool UseLibuv { get; set; } = true;

        public string SpecialAddress { get; set; } = "127.0.0.1";

        public string StartupWords { get; set; } = "TcpServerHost bind at {bindAddress}";
    }

    public enum AddressBindType
    {
        Any, //0.0.0.0 Address.Any
        Loopback, // localhost
        InternalAddress, //本机内网地址
        SpecialAddress //自定义地址
    }
}
