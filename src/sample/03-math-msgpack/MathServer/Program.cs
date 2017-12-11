using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using DotBPE.Rpc.Extensions;
using DotBPE.Rpc.Hosting;
using DotBPE.Rpc.Logging;
using MessagePack;
using MathCommon;
using Microsoft.Extensions.DependencyInjection;

namespace MathServer
{
    class Program
    {
        static void Main(string[] args)
        {

            //DotBPE.Rpc.Environment.SetLogger(new DotBPE.Rpc.Logging.ConsoleLogger());

            string ip = "127.0.0.1";
            int port = 6201;

            var host = new RpcHostBuilder()
                .UseServer(ip, 6201)
                .UseStartup<ServerStartup>()
                .Build();

            host.StartAsync().Wait();
            Console.WriteLine("running server on {0}:{1}" , ip, port);
            Console.WriteLine("press any key to shutdown");
            Console.ReadKey();

            host.ShutdownAsync().Wait();
            Console.WriteLine("server is shutdown");
        }
    }

    public class MathService : ServiceActor
    {
        static ILogger Logger = DotBPE.Rpc.Environment.Logger.ForType<MathService>();

        /// <summary>
        /// 服务的标识,这里的服务号是10001
        /// </summary>
        protected override int ServiceId => 10002;

        /// <summary>
        /// 处理消息请求
        /// </summary>
        /// <param name="req">请求的消息</param>
        /// <returns>返回消息</returns>
        public override Task<AmpMessage> ProcessAsync(AmpMessage req)
        {
            Logger.Debug("receive message:ServiceId={0},MessageId = {1},Sequence={2}",req.ServiceId,req.MessageId,req.Sequence) ;
            switch(req.MessageId){
                case 1: //  1 = Add
                    return  AddAsync(req);
                default:
                    return base.ProcessNotFoundAsync(req);
            }
        }

        public Task<AmpMessage> AddAsync(AmpMessage reqMsg){

            var rsp = AmpMessage.CreateResponseMessage(reqMsg.ServiceId, reqMsg.MessageId);

            AddReq req =  MessagePackSerializer.Deserialize<AddReq>(reqMsg.Data);

            if(req !=null){
                var res = new AddRes();
                res.C  = req.A + req.B ;

                rsp.Data = MessagePackSerializer.Serialize(res);

                Logger.Debug("{0}${1}${2}  ,code=0  ,req={3}  ,res={4}",reqMsg.ServiceId,reqMsg.MessageId,reqMsg.Sequence, MessagePackSerializer.ToJson(req), MessagePackSerializer.ToJson(res));
            }
            else{
                rsp.Code = ErrorCodes.CODE_INTERNAL_ERROR;
                Logger.Error("{0}${1}${2}  ,code={3}  ,req={4}  ,res=null",reqMsg.ServiceId,reqMsg.MessageId,reqMsg.Sequence,rsp.Code, MessagePackSerializer.ToJson(req));
            }
            return Task.FromResult(rsp);
        }
    }

    public class ServerStartup : IStartup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddDotBPE(); // 使用AMP协议

            services.AddServiceActors<AmpMessage>((actors) =>
            {
                actors.Add<MathService>();
            });

            return services.BuildServiceProvider();
        }

        public void Configure(IAppBuilder app, IHostingEnvironment env)
        {

        }

    }
}
