using DotBPE.Protocol.Amp;
using System;
using System.Text;
using MathCommon;
using Google.Protobuf;
using System.Threading.Tasks;

namespace MathClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(RunClient).Wait();
        }

        public async static Task RunClient()
        {
            using (var client = new MathCommon.MathClient("127.0.0.1:6201"))
            {
                Console.WriteLine("ready to send message");

                var random = new Random();

                AddReq req = new AddReq
                {
                    A = random.Next(1, 10000),
                    B = random.Next(1, 10000)
                };

                Console.WriteLine("call sever MathService.Add  --> {0}+{1} ", req.A, req.B);

                try
                {
                    var res = await client.AddAsync(req);
                    Console.WriteLine("server repsonse:<-----{0}+{1}={2}", req.A, req.B, res.Data.C);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("error occ {0}", ex.Message);
                }

            }
            Console.WriteLine("channel is closed!");
        }
    }
}
