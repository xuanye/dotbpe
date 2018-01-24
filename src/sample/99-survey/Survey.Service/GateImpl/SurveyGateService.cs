using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using Survey.Core;
using System.Threading.Tasks;

namespace Survey.Service.GateImpl
{
    public class SurveyGateService : SurveyGateServiceBase
    {
        private readonly ClientProxy _proxy;
        public SurveyGateService(ClientProxy proxy)
        {
            this._proxy = proxy;
        }
        public override Task<RpcResult<APaperRsp>> GetAPaperAsync(GetAPaperReq request)
        {
            var client = _proxy.GetClient<APaperInnerServiceClient>();
            return client.GetAPaperAsync(request);
        }

        public override async Task<RpcResult<QPaperStaRsp>> GetAPaperStaAsync(GetQPaperStaReq request)
        {
            var res = new RpcResult<QPaperStaRsp>();
            res.Data = new QPaperStaRsp();
            if (string.IsNullOrEmpty(request.Identity))
            {
                res.Code = ErrorCodes.AUTHORIZATION_REQUIRED;
                res.Data.ReturnMessage = "请先登录";
                return res;
            }
                       
            var ap_client = _proxy.GetClient<APaperInnerServiceClient>();
            var qp_client = _proxy.GetClient<QPaperInnerServiceClient>();

            var req1 = new GetAPaperStaDetailReq();
            req1.ClientIp = request.ClientIp;
            req1.Identity = request.Identity;
            req1.XRequestId = request.XRequestId;
            req1.QpaperId = request.QpaperId;

            var t1 = ap_client.GetAPaperStaAsync(req1);
            var req2 = new GetQPaperReq();
            req2.ClientIp = request.ClientIp;
            req2.Identity = request.Identity;
            req2.XRequestId = request.XRequestId;
            req2.QpaperId = request.QpaperId;
            var t2 = qp_client.GetQPaperFullAsync(req2);

            await Task.WhenAll(t1, t2);

            if(t1.Result.Code !=0)
            {
                res.Code = t1.Result.Code;
                res.Data.ReturnMessage = t1.Result.Data.ReturnMessage;
                return res;
            }
            if (t2.Result.Code != 0)
            {
                res.Code = t2.Result.Code;
                res.Data.ReturnMessage = t2.Result.Data.ReturnMessage;
                return res;
            }

            res.Data.Qpaper = t2.Result.Data.Qpaper;
            res.Data.StaDetail.AddRange(t1.Result.Data.StaDetail);

            return res;
        }

        public override Task<RpcResult<QPaperRsp>> GetQPaperAsync(GetQPaperReq request)
        {
            var client = _proxy.GetClient<QPaperInnerServiceClient>();
            return client.GetQPaperAsync(request);
        }

        public override Task<RpcResult<QPaperFullRsp>> GetQPaperFullAsync(GetQPaperReq request)
        {
            var client = _proxy.GetClient<QPaperInnerServiceClient>();
            return client.GetQPaperFullAsync(request);
        }

        public override Task<RpcResult<APaperListRsp>> QueryAPaperListAsync(QueryAPaperReq request)
        {
            var res = new RpcResult<APaperListRsp>();
            res.Data = new APaperListRsp();
            if (string.IsNullOrEmpty(request.Identity))
            {
                res.Code = ErrorCodes.AUTHORIZATION_REQUIRED;
                res.Data.ReturnMessage = "请先登录";
                return Task.FromResult(res);
            }

            var client = _proxy.GetClient<APaperInnerServiceClient>();
            return client.QueryAPaperListAsync(request);
        }

        public override Task<RpcResult<QPaperListRsp>> QueryQPaperListAsync(QueryQPaperReq request)
        {
            var res = new RpcResult<QPaperListRsp>();
            res.Data = new QPaperListRsp();
            if (string.IsNullOrEmpty(request.Identity))
            {
                res.Code = ErrorCodes.AUTHORIZATION_REQUIRED;
                res.Data.ReturnMessage = "请先登录";
                return Task.FromResult(res);
            }

            var client = _proxy.GetClient<QPaperInnerServiceClient>();
            return client.QueryQPaperListAsync(request);
        }

        public override Task<RpcResult<SaveAPaperRsp>> SaveAPaperAsync(SaveAPaperReq request)
        {
            var client = _proxy.GetClient<APaperInnerServiceClient>();
            return client.SaveAPaperAsync(request);
        }

        public override Task<RpcResult<SaveQPaperRsp>> SaveQPaperAsync(SaveQPaperReq request)
        {
            var res = new RpcResult<SaveQPaperRsp>();
            res.Data = new SaveQPaperRsp();
            if (string.IsNullOrEmpty(request.Identity))
            {
                res.Code = ErrorCodes.AUTHORIZATION_REQUIRED;
                res.Data.ReturnMessage = "请先登录";
                return Task.FromResult(res);
            }


            var client = _proxy.GetClient<QPaperInnerServiceClient>();

            return client.SaveQPaperAsync(request);
        }

        
    }
}
