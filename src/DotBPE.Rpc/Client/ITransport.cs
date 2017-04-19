using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;

namespace DotBPE.Rpc
{
    public interface ITransport<in TMessage> where TMessage :InvokeMessage
    {
        Task SendAsync(TMessage request);
        Task CloseAsync();
    }
}
