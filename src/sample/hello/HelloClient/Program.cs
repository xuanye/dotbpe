using DotBPE.Protocol.Amp;

using System;
using System.Text;


namespace HelloClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.OutputEncoding = Encoding.UTF8;
            //DotBPE.Rpc.Environment.SetLogger(new DotBPE.Rpc.Logging.ConsoleLogger());
        

            using (var caller = new AmpCallInvoker("127.0.0.1:6201"))
            {
                Console.WriteLine("服务器已连接!");
                ushort serviceId = 10000;
                AmpMessage req = AmpMessage.CreateRequestMessage(serviceId, 0);
                req.Data = Encoding.UTF8.GetBytes("dotbpe");

                Console.WriteLine("send name:dotbpe to sever-->");
                var res = caller.AsyncCall(req).Result;

                if (res.Code == 0)
                {
                    string hello = Encoding.UTF8.GetString(res.Data);
                    Console.WriteLine("server repsonse:<-----{0}", hello);
                }
                else
                {
                    Console.WriteLine("server error,code={0}", res.Code);
                }
            }
            Console.WriteLine("服务器连接已断开!");

        }
    }
}
