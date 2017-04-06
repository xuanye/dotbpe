using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc
{
    public interface IServerHost:IDisposable
    {
        Task StartAsync(EndPoint endpoint);
    }
}
