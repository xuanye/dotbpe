using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using DotBPE.Rpc.Extensions;
using DotBPE.Rpc.Hosting;
using MathCommon;
using Microsoft.Extensions.DependencyInjection;
using Google.Protobuf;
using DotBPE.Hosting;

namespace MathServer
{
    class Program
    {
        static void Main(string[] args)
        {

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
                    actors.Add<MathService>();
                });

                //添加挂载的宿主服务
                services.AddScoped<IHostedService, RpcHostedService>();
            });



            builder.RunConsoleAsync().Wait();

        }
    }

    public class MathService : MathBase
    {
        public override Task<RpcResult<AddRes>> AddAsync(AddReq req){
            var res = new AddRes();
            res.C  = req.A + req.B ;
            return Task.FromResult(new RpcResult<AddRes>() { Data = res });
        }
    }

}
