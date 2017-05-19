using DotBPE.Rpc.DefaultImpls;
using DotBPE.Rpc.Options;
using Microsoft.Extensions.Options;

namespace DotBPE.Protocol.Amp
{
    /// <summary>
    /// 通过ServiceId和MessageId在Config中查找配置
    /// </summary>
    public class AmpBridgeRouter : LocalConfigBridgeRouter<AmpMessage>
    {
        public AmpBridgeRouter(IOptions<RemoteServicesOption> options) : base(options)
        {
        }

        protected override string GetMessageKey(AmpMessage message)
        {
            return string.Format("{0}${1}",message.ServiceId,message.MessageId);
        }

        protected override string GetServiceKey(AmpMessage message)
        {
            return string.Format("{0}$0", message.ServiceId);
        }
    }
}