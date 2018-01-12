using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using Survey.Core;
using System.Threading.Tasks;

namespace Survey.Service.GateImpl
{
    public class SurveyGateService : SurveyGateServiceBase
    {
       
        public override Task<RpcResult<APaperRsp>> GetAPaperAsync(GetAPaperReq request)
        {
            var client = ClientProxy.GetClient<APaperInnerServiceClient>();
            return client.GetAPaperAsync(request);
        }

        public override Task<RpcResult<QPaperRsp>> GetQPaperAsync(GetQPaperReq request)
        {
            var client = ClientProxy.GetClient<QPaperInnerServiceClient>();
            return client.GetQPaperAsync(request);
        }

        public override Task<RpcResult<APaperListRsp>> QueryAPaperListAsync(QueryAPaperReq request)
        {
            var client = ClientProxy.GetClient<APaperInnerServiceClient>();
            return client.QueryAPaperListAsync(request);
        }

        public override Task<RpcResult<QPaperListRsp>> QueryQPaperListAsync(QueryQPaperReq request)
        {
            var client = ClientProxy.GetClient<QPaperInnerServiceClient>();
            return client.QueryQPaperListAsync(request);
        }

        public override Task<RpcResult<SaveAPaperRsp>> SaveAPaperAsync(SaveAPaperReq request)
        {
            var client = ClientProxy.GetClient<APaperInnerServiceClient>();
            return client.SaveAPaperAsync(request);
        }

        public override Task<RpcResult<SaveQPaperRsp>> SaveQPaperAsync(SaveQPaperReq request)
        {
            var client = ClientProxy.GetClient<QPaperInnerServiceClient>();
            return client.SaveQPaperAsync(request);
        }
    }
}
