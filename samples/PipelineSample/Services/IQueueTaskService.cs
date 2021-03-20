using DotBPE.Rpc;
using DotBPE.Rpc.Server;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSample.Services
{
    [RpcService(101)]
    public interface IQueueTaskService
    {
        [RpcMethod(1)]
        Task<RpcResult<VoidRes>> DoWork(QueueTaskReq req);
    }

    public class QueueTaskService : BaseService<IQueueTaskService>, IQueueTaskService
    {      
      

        public async Task<RpcResult<VoidRes>> DoWork(QueueTaskReq req)
        {
            Logger.LogInformation("{0}: do work {1} ,after {2} seconds,data:\r\n-----\r\n{3}\r\n"
                , DateTime.Now.ToString("yyyyMMdd HH:mm:ss"), req.XRequestId, req.Delay, req.JobData);

            //await Task.Delay(1000);
            Console.WriteLine("*******************************************");
            Console.WriteLine($"**********执行任务:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}");
            // throw new Exception("********任务错了********");
            await Task.CompletedTask;

            return new RpcResult<VoidRes> { Data = new VoidRes { ReturnMessage = "Hello" } };
        }
    }
}
