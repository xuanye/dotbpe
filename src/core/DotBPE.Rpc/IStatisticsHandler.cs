using System.Net;
using DotBPE.Rpc.Codes;

namespace DotBPE.Rpc
{
    public interface IStatisticsHandler<TMessage> where TMessage :InvokeMessage
    {
        void BeforeSend(EndPoint remote,TMessage message);

        void AfterReceive(EndPoint remote,TMessage message);

    }
}
