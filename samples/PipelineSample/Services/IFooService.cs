using DotBPE.Extra;
using DotBPE.Gateway;
using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Server;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSample.Services
{
    [RpcService(100)]
    public interface IFooService
    {
        [RpcMethod(1)]
        [Router("/api/task/enqueue")]
        Task<RpcResult<VoidRes>> Enqueue(QueueTaskReq req);
    }

    public class FooService : BaseService<IFooService>, IFooService
    {
        private readonly IPipelineTaskService _pipeline;
      
        private readonly IClientProxy _proxy;

        public FooService(IPipelineTaskService pipeline,  IClientProxy proxy)
        {
            _pipeline = pipeline;            
            _proxy = proxy;
        }

        public async Task<RpcResult<VoidRes>> Enqueue(QueueTaskReq req)
        {
            RpcResult<VoidRes> result = new RpcResult<VoidRes> { Data = new VoidRes() };

            //req.JobData = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var qService = _proxy.Create<IQueueTaskService>();
            if (req.Delay <= 0)
            {
                Logger.LogInformation("{0}:receive queue task ,enqueue!", DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));
                var r1 = await _pipeline.Enqueue(qService.DoWork, req);
                result.Code = r1.Code;
                result.Data.ReturnMessage = "receive queue task ,enqueue!";
            }
            else
            {
                Logger.LogInformation("{0}:receive delay queue task ,enqueue!", DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));
                var r2 = await _pipeline.EnqueueDelay(qService.DoWork, req, TimeSpan.FromSeconds(req.Delay));
                result.Code = r2.Code;
                result.Data.ReturnMessage = "receive delay queue task ,enqueue!";
            }

            return result;
        }
    }
}
