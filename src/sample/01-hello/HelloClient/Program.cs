using DotBPE.Protocol.Amp;

using System;
using System.Text;


namespace HelloClient
{
    class Program
    {
        static void Main(string[] args)
        {

            using (var caller = new AmpCallInvoker("127.0.0.1:6201"))
            {
                Console.WriteLine("ready to send message");
                ushort serviceId = 10000;
                AmpMessage req = AmpMessage.CreateRequestMessage(serviceId, 0);
                req.Data = Encoding.UTF8.GetBytes("dotbpe");

                Console.WriteLine("send name:dotbpe to sever-->");

                try
                {
                    var res = caller.BlockingCall(req);

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
                catch(Exception ex)
                {
                    Console.WriteLine("error occ {0}", ex.Message);
                }

            }
            Console.WriteLine("channel is closed!");

        }
    }
}
