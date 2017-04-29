using System;
using System.Text;
using DotBPE.Protocol.Amp;
using HelloRpc.Common;

namespace HelloRpc.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            //NLoggerWrapper.InitConfig();
            //DotBPE.Rpc.Environment.SetLogger(new NLoggerWrapper(typeof(Program)));

            var client = AmpClient.Create("127.0.0.1:6201");
            var greeter = new GreeterClient(client);
            int i = 0;
            while (i<100)
            {
                //Console.WriteLine("请输入你的名称");
                string name ="xuanye"; //Console.ReadLine();
                if ("bye".Equals(name))
                {
                    break;
                }
                try
                {
                    var request =new HelloRequest(){Name = name};
                    greeter.HelloAsync(request).ContinueWith(task=>{
                        Console.WriteLine($"---------------收到服务端返回:{task.Result.Message}-----------");
                    });
                    i++;
                }
                catch(Exception ex){
                    Console.WriteLine("发生错误："+ex.Message);
                }
            }
            Console.ReadKey();
        }
    }


}
