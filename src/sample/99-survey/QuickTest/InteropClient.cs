using CommandLine;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc.Utils;
using Google.Protobuf;
using Survey.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace QuickTest
{
    public class InteropClient
    {
        private static readonly JsonFormatter AmpJsonFormatter = new JsonFormatter(new JsonFormatter.Settings(true));

        private class ClientOption
        {
            [Option("server", Default = "127.0.0.1:5501")]
            public string Server { get; set; }

            [Option("testcase", Default = "default")]
            public string TestCase { get; set; }

            [Option("mpc", Default = 1)]
            public int MultiplexCount { get; set; }
        }

        private readonly ClientOption _options;

        private InteropClient(ClientOption options)
        {
            this._options = options;
        }

        private static int TOTAL_ERROR = 0;

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
                interopClient.Run();
                swTotal.Stop();
                Console.WriteLine("---------------------Summary:--------------------------");
                Console.WriteLine("Error times: {0}", TOTAL_ERROR);
                Console.WriteLine("Elapsed time: {0}ms", swTotal.ElapsedMilliseconds);
                Console.ReadKey();
            });
        }

        private void Run()
        {
            //读取测试的文件
            var tcFilePath = Path.Combine(CommonUtils.GetAppRootPath(), "testcase", this._options.TestCase + ".txt");

            if (!File.Exists(tcFilePath))
            {
                Console.WriteLine(tcFilePath);
                Console.WriteLine("对应的测试文件不存在");
                System.Environment.Exit(1);
                return;
            }
            Dictionary<string, TestCase> tcCache = new Dictionary<string, TestCase>();
            using (StreamReader reader = File.OpenText(tcFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }
                    if (line.StartsWith("#"))
                    {
                        continue;
                    }

                    var tc = ParseFromLine(line);
                    tcCache.Add(tc.Id, tc);
                }
            }

            //创建链接
            var client = AmpClient.Create(this._options.Server, this._options.MultiplexCount);
            var caller = new AmpCallInvoker(client);

            //开始跑测试了
            foreach (var kvtc in tcCache)
            {
                RunTestCase(caller, kvtc.Value);
            }
            client.CloseAsync().Wait();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="tc"></param>
        private void RunTestCase(AmpCallInvoker caller, TestCase tc)
        {
            try
            {
                Console.WriteLine("--------start run testcase {0}-----------", tc.Id);
                Console.WriteLine("ServiceId: {0},MessageId:{1},Req:{2}", tc.ServiceId, tc.MessageId, tc.Json);

                //构造请求消息
                AmpMessage message = AmpMessage.CreateRequestMessage(tc.ServiceId, tc.MessageId);
                tc.Request = ProtobufObjectFactory.GetRequestTemplate(tc.ServiceId, tc.MessageId);

                if (tc.Request == null)
                {
                    TOTAL_ERROR++;
                    Console.WriteLine("执行测试出错:不存在对应的服务，请检查后重试");
                    System.Environment.Exit(1);
                    return;
                }
                var descriptor = tc.Request.Descriptor;
                tc.Request = descriptor.Parser.ParseJson(tc.Json);
                message.Data = tc.Request.ToByteArray();
                //请求消息构造完毕

                AmpMessage rsp = caller.BlockingCall(message);

                if (rsp == null)
                {
                    TOTAL_ERROR++;
                    Console.WriteLine(">>----end run testcase {0} fail,no repsonse--------", tc.Id);
                }
                else
                {
                    Console.WriteLine(">>----end run testcase {0} success,response code = {1}-------", tc.Id, rsp.Code);

                    var jsonRsp = MessageToJson(tc, rsp);
                    if (string.IsNullOrEmpty(jsonRsp))
                    {
                        TOTAL_ERROR++;
                        Console.WriteLine(">>----end run testcase {0} fail,没有配置对应服务响应消息---------", tc.Id);
                    }
                    else
                    {
                        Console.WriteLine(">>response:{0}", jsonRsp);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("执行测试出错:" + ex.Message);
                System.Environment.Exit(1);
            }
        }

        private TestCase ParseFromLine(string line)
        {
            TestCase tc = new TestCase();

            if (line.StartsWith("$"))
            {
                var tcData = line.Split(new char[] { ',' }, 4);
                tc.Id = tcData[0].Substring(1);
                tc.ServiceId = ushort.Parse(tcData[1]);
                tc.MessageId = ushort.Parse(tcData[2]);
                tc.Json = tcData[3];
            }
            else
            {
                tc.Id = Guid.NewGuid().ToString("N");
                var tcData = line.Split(new char[] { ',' }, 3);
                tc.ServiceId = ushort.Parse(tcData[0]);
                tc.MessageId = ushort.Parse(tcData[1]);
                tc.Json = tcData[2];
            }

            return tc;
        }

        private string MessageToJson(TestCase tc, AmpMessage message)
        {
            if (message.Code == 0)
            {
                var return_code = 0;
                var return_message = "";
                string ret = "";
                if (message != null)
                {
                    tc.Response = ProtobufObjectFactory.GetResponseTemplate(message.ServiceId, message.MessageId);
                    if (tc.Response == null)
                    {
                        return ret;
                    }

                    if (message.Data != null)
                    {
                        tc.Response.MergeFrom(message.Data);
                    }
                    //提取内部的return_code 字段
                    var field_code = tc.Response.Descriptor.FindFieldByName("return_code");
                    if (field_code != null)
                    {
                        var retObjV = field_code.Accessor.GetValue(tc.Response);
                        if (retObjV != null)
                        {
                            if (!int.TryParse(retObjV.ToString(), out return_code))
                            {
                                return_code = 0;
                            }
                        }
                    }
                    //提取return_message
                    var field_msg = tc.Response.Descriptor.FindFieldByName("return_message");
                    if (field_msg != null)
                    {
                        var retObjV = field_msg.Accessor.GetValue(tc.Response);
                        if (retObjV != null)
                        {
                            return_message = retObjV.ToString();
                        }
                    }

                    ret = AmpJsonFormatter.Format(tc.Response);
                }

                return "{\"return_code\":" + return_code + ",\"return_message\":\"" + return_message + "\",data:" + ret + "}";
            }
            else
            {
                return "{\"return_code\":" + message.Code + ",\"return_message\":\"\"}";
            }
        }
    }
}
