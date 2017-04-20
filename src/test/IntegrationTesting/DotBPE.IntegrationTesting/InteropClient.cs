using System;
using System.Threading.Tasks;
using CommandLine;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;

namespace DotBPE.IntegrationTesting
{
    public class InteropClient
    {
        private class ClientOption
        {

            [Option("server", Default = "127.0.0.1:6201")]
            public string Server { get; set; }

            [Option("testcase", Default = "echo")]
            public string TestCase { get; set; }

            [Option("rtc", Default = 1)]
            public int RunThreadCount {get;set;}

            [Option("rtc", Default = 1)]
            public int RunCount {get;set;}

            [Option("mpc", Default = 5)]
            public int MultiplexCount{get;set;}

        }

        private readonly ClientOption _options;

        private InteropClient(ClientOption options)
        {
            this._options = options;
        }

        public static void Run(string[] args)
        {
            var parserResult = Parser.Default.ParseArguments<ClientOption>(args)
            .WithNotParsed(errors => System.Environment.Exit(1))
            .WithParsed(options =>
            {
                var interopClient = new InteropClient(options);
                interopClient.Run().Wait();
            });
        }

        private Task Run()
        {
            var tasks = new Task[ this._options.RunThreadCount];
            for(int i = 0 ; i< this._options.RunThreadCount ;i++){
                tasks[i] = Task.Factory.StartNew( async ()=>{
                    var client = AmpClient.Create(this._options.Server,this._options.MultiplexCount);
                    await RunTestCaseAsync(client);
                    await client.CloseAsync();
                });
            }
            return Task.WhenAll(tasks);

        }

        private Task RunTestCaseAsync(IRpcClient<AmpMessage> client)
        {
            switch(this._options.TestCase){
                case "benchmark":
                    return RunBenchmarkTestCaseAsync(client);
                default:
                    Console.WriteLine("TestCase not implemented");
                    break;
            }
            return Task.CompletedTask;
        }

        private async Task RunBenchmarkTestCaseAsync(IRpcClient<AmpMessage> client)
        {
            var msg = new BenchmarkMessage();
            msg.Field1 = "REQUEST";
            msg.Field2 = 1;
            BenchmarkTestClient btc = new BenchmarkTestClient(client);
            for(int i =0;i<this._options.RunCount ; i++){
               await btc.EchoAsnyc(msg);
            }
        }
    }
}
