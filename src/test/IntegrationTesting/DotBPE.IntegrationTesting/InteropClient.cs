using System;
using System.Diagnostics;
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

            [Option("rc", Default = 1)]
            public int RunCount {get;set;}

            [Option("mpc", Default = 5)]
            public int MultiplexCount{get;set;}

        }

        private readonly ClientOption _options;

        private InteropClient(ClientOption options)
        {
            this._options = options;
        }
        private static int TOTAL_ERROR =0;
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
                Console.WriteLine("Start to Run!");
                TOTAL_ERROR = 0;
                var swTotal = new Stopwatch();
                swTotal.Start();
                interopClient.Run().Wait();
                swTotal.Stop();
                Console.WriteLine("--------------------- total --------------------------------");
                Console.WriteLine("Error times: {0}", TOTAL_ERROR);
                Console.WriteLine("Elapsed time: {0}ms", swTotal.ElapsedMilliseconds);
                Console.WriteLine("Ops per second: {0}", (int)((double)options.RunCount*options.RunThreadCount  * 1000 / swTotal.ElapsedMilliseconds));
                Console.WriteLine("press any key to quit!");
                Console.ReadKey();
            });
        }

        private Task Run()
        {
            var tasks = new Task[ this._options.RunThreadCount];
            for(int i = 0 ; i< this._options.RunThreadCount ;i++){

                tasks[i] = Task.Factory.StartNew( async (index)=>{
                    var client = AmpClient.Create(this._options.Server,this._options.MultiplexCount);

                    await RunTestCaseAsync(client,(int)index);
                    await client.CloseAsync();
                    Console.WriteLine("close client {0}", (int)index);
                },i);
            }
            return Task.WhenAll(tasks);
        }

        private Task RunTestCaseAsync(IRpcClient<AmpMessage> client,int threadIndex)
        {
            switch(this._options.TestCase){
                case "benchmark":
                    Console.WriteLine("Run TestCase Benchmark!");
                    return RunBenchmarkTestCaseAsync(client,threadIndex);
                case "callcontexttest":
                    Console.WriteLine("Run TestCase CallContextTest!");
                    return RunCallContextTestCaseAsync(client, threadIndex);
                default:
                    Console.WriteLine("TestCase not implemented");
                    break;
            }
            return Task.CompletedTask;
        }
        private static BenchmarkMessage PrepareBenchmarkMessage(){
            var msg = new BenchmarkMessage();
            msg.Field1 = "REQUEST";
            msg.Field2 = 1;
            return msg;
        }
        private Task RunCallContextTestCaseAsync(IRpcClient<AmpMessage> proxy, int threadIndex)
        {
            int errorCount = 0;
            int callCount = 0;
            CallContextTestClient cctClient = new CallContextTestClient(proxy);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var req = new VoidReq();
            for (int i = 0; i < this._options.RunCount; i++)
            {
                var rsp = cctClient.TestAsync(req).Result;
                if (rsp.Status != 0)
                {
                    TOTAL_ERROR++;
                    errorCount++;
                }
                callCount++;
            }
            stopwatch.Stop();
            Console.WriteLine("--------------------- result {0}--------------------------------", threadIndex);
            Console.WriteLine("Call TestAsync {0} times,Error times: {1}", callCount, errorCount);
            Console.WriteLine("Elapsed time: {0}ms", stopwatch.ElapsedMilliseconds);
            Console.WriteLine("Ops per second: {0}", (int)((double)this._options.RunCount * 1000 / stopwatch.ElapsedMilliseconds));          
            return Task.CompletedTask;
        }

        private Task RunBenchmarkTestCaseAsync(IRpcClient<AmpMessage> proxy, int threadIndex)
        {
            var stopwatch = new Stopwatch();

            var msg = PrepareBenchmarkMessage();
            int errorCount = 0;
            int callCount = 0;
            BenchmarkTestClient btc = new BenchmarkTestClient(proxy);
            stopwatch.Start();
            for(int i =0;i<this._options.RunCount ; i++){
                var rsp = btc.EchoAsync(msg,6000000).Result;
                if(rsp.Field2 !=100){
                    TOTAL_ERROR++;
                    errorCount ++;
                }
                callCount++;
            }
            stopwatch.Stop();
            Console.WriteLine("--------------------- result {0}--------------------------------",threadIndex);
            Console.WriteLine("Call Echo {0} times,Error times: {1}",callCount, errorCount);
            Console.WriteLine("Elapsed time: {0}ms", stopwatch.ElapsedMilliseconds);
            Console.WriteLine("Ops per second: {0}", (int)((double)this._options.RunCount  * 1000 / stopwatch.ElapsedMilliseconds));
            //await btc.QuitAsnyc(new Void());
            return Task.CompletedTask;
        }
    }
}
