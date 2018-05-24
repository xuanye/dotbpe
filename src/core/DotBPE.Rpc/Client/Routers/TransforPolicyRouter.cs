using DotBPE.Rpc.Options;
using DotBPE.Rpc.Utils;
using Microsoft.Extensions.Options;

namespace DotBPE.Rpc.Client
{
    public class TransforPolicyRouter<TMessage> : IRouter<TMessage> where TMessage : InvokeMessage
    {
        public TransforPolicyRouter(IOptions<RpcClientOption> clientOption)
        {
            Preconditions.CheckNotNull(clientOption, "未配置默认地址");
            Preconditions.CheckNotNull(clientOption.Value, "未配置默认地址");
            _defaultRout = new RouterPoint();
            _defaultRout.RemoteAddress = ParseUtils.ParseEndPointFromString(clientOption.Value.DefaultServerAddress);
            _defaultRout.RoutePointType = RoutePointType.Remote;
        }

        private readonly RouterPoint _defaultRout;

        public RouterPoint GetRouterPoint(TMessage message)
        {
            return _defaultRout;
        }
    }
}
