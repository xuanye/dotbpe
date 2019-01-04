using System;
using System.Threading.Tasks;
using CommandLine;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using PiggyMetrics.Common;
using DotBPE.Rpc.Logging;

namespace PiggyMetrics.ConsoleClient
{
    public class InteropClient
    {
        static readonly ILogger Logger = DotBPE.Rpc.Environment.Logger.ForType<InteropClient>();
        private class ClientOption
        {

            [Option("server", Default = "127.0.0.1:6201")]
            public string Server { get; set; }

            [Option("testcase", Default = "echo")]
            public string TestCase { get; set; }

            [Option("mpc", Default = 5)]
            public int MultiplexCount { get; set; }

        }

        private readonly ClientOption _options;

        private InteropClient(ClientOption options)
        {
            this._options = options;
        }

        public static void Run(string[] args)
        {
            var parserResult = Parser.Default.ParseArguments<ClientOption>(args)
            .WithNotParsed(errors =>
            {
                Console.WriteLine(errors);
                System.Environment.Exit(1);
            })
            .WithParsed(options =>
            {
                var interopClient = new InteropClient(options);
                Logger.Debug("Start to Run!");
                interopClient.Run().Wait();
                Console.WriteLine("press any key to quit!");
                Console.ReadKey();
            });
        }

        private async Task Run()
        {
            var client = AmpClient.Create(this._options.Server, this._options.MultiplexCount);
            await RunTestCaseAsync(client);
            await client.CloseAsync();
        }

        private Task RunTestCaseAsync(IRpcClient<AmpMessage> client)
        {
            switch (this._options.TestCase)
            {
                case "auth_create":
                    Logger.Debug("Run TestCase auth_create!");
                    return RunAuthCreateTestCaseAsync(client);
                default:
                    Logger.Warning("TestCase not implemented");
                    break;
            }
            return Task.CompletedTask;
        }

        private async Task RunAuthCreateTestCaseAsync(IRpcClient<AmpMessage> client)
        {
            string random = Guid.NewGuid().ToString("D");
            var msg = new UserReq()
            {
                Account = random.Substring(10),
                Password = random
            };

            AuthServiceClient proxy = new AuthServiceClient(client);
            var rsp = await proxy.CreateAsync(msg, 6000000);

            if(rsp == null)
            {
                Logger.Error("response message is null！");
            }
            else if(rsp.Status != 0)
            {
                Logger.Error("return fail:{0}",rsp.Message);
            }
            Logger.Debug("AuthCreateTest Success");
        }
    }
}
