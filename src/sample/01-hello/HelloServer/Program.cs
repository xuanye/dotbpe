using System;
using System.Text;
using System.Threading.Tasks;
using DotBPE.Hosting;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using DotBPE.Rpc.Extensions;
using DotBPE.Rpc.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace HelloServer
{
    class Program
    {
        static void Main(string[] args)
        {

            //DotBPE.Rpc.Environment.SetLogger(new DotBPE.Rpc.Logging.ConsoleLogger());

            string ip = "127.0.0.1";
            int port = 6201;

            var builder = new HostBuilder()
                .UseServer(ip, port)
                .ConfigureServices(services =>
               {
                   //添加协议支持
                   services.AddDotBPE();

                   //注册服务
                   services.AddServiceActors<AmpMessage>((actors) =>
                   {
                       actors.Add<HelloService>();
                   });

                   //添加挂载的宿主服务
                   services.AddScoped<IHostedService, RpcHostedService>();
               });



            builder.RunConsoleAsync().Wait();

        }
    }

    public class HelloService : ServiceActor
    {
        /// <summary>
        /// 服务的标识,这里的服务号是10000
        /// </summary>
        protected override int ServiceId => 10000;

        /// <summary>
        /// 处理消息请求
        /// </summary>
        /// <param name="req">请求的消息</param>
        /// <returns>返回消息</returns>
        public override Task<AmpMessage> ProcessAsync(AmpMessage req)
        {
            var rsp = AmpMessage.CreateResponseMessage(req.ServiceId, req.MessageId);

            var name = Encoding.UTF8.GetString(req.Data);

            rsp.Data = Encoding.UTF8.GetBytes(string.Format("Hello {0}！", name));
            return Task.FromResult(rsp);
        }
    }
    
}
