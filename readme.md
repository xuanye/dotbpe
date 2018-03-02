# dotbpe

---


dotbpe ![](https://travis-ci.org/xuanye/dotbpe.svg?branch=master)
-------------
dotbpe是一款基于DOTNET Core编写的RPC框架，但是它的目标不仅仅只是解决rpc的问题，而是解决整个业务解决方案的问题，封装在常见的项目产品开发中的通用组件，让开发人员只专注于开发业务逻辑代码。底层通信默认实现基于DotNetty，可替换成其他Socket通信组件。dotbpe使用的默认协议名称叫Amp,编解码使用谷歌的Protobuf3,不过这些默认实现都是可以替换的。




## 快速开始


### 实现一个HelloWorld的Rpc服务调用

#### 服务端实现

 1. 新建控制台程序 HelloServer
 2. 添加服务端的引用

```shell
$> dotnet add package DotBPE.Rpc //DotBPE的核心服务
$> dotnet add package DotBPE.Rpc.Hosting //挂载DotBPE的Host扩展
$> dotnet add package DotBPE.Rpc.Netty //使用Netty作为通讯框架
$> dotnet add package DotBPE.Protocol.Amp //使用默认的AMP协议
```

3. 代码清单：

第一步，编写一个类用于实现Hello的消息响应,这里数据使用UTF8字符串编码，代码

```CSharp
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

```
第二部 启动服务

```CSharp
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
            string ip = "0.0.0.0";
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


```

#### 客户端实现
 1. 新建控制台程序 HelloClient
 2. 添加客户端相关的引用

 ```shell
$> dotnet add package DotBPE.Rpc //DotBPE的核心服务
$> dotnet add package DotBPE.Rpc.Netty //使用Netty作为通讯框架
$> dotnet add package DotBPE.Protocol.Amp //使用默认的AMP协议
```

3. 代码清单

```CSharp

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
                    var res = caller.BlockingCall(req); //同步调用
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
```

### 查看运行效果


首先运行服务端，图片右侧，再运行客户端，查看效果，如下图所示：
![](http://ww1.sinaimg.cn/large/697065c1ly1fnswehxgaxj20tf08rq4d.jpg)


详细代码请查看：[HelloDotBPE示例][1]

## 其他示例
代码中还提供了[json编码][2]，[msgpack编码][3]和[protobuf编码][4]方式的示例

## 使用ProtoBuf 定义RPC服务描述文件，并自动生成相关客户端和服务端基础代码

1. 定义服务描述的扩展信息文件

该文件可以从 [此处][5] 获取，复制到你的项目中即可

```protobuf

// [START declaration]
syntax = "proto3";
package dotbpe;
// [END declaration]

// [START csharp_declaration]
option csharp_namespace = "DotBPE.Protobuf";
// [END csharp_declaration]

import "google/protobuf/descriptor.proto";

//扩展服务
extend google.protobuf.ServiceOptions {
  int32 service_id = 51001; //服务号
  bool disable_generic_service_client = 51003; //是否生成客户端代码
  bool disable_generic_service_server = 51004; //是否生成服务端代码
}
extend google.protobuf.MethodOptions {
  int32 message_id = 51002; //消息号

  HttpApiOption http_api_option = 51003; //http 接口的对应配置
}

extend google.protobuf.FileOptions {
  bool disable_generic_services_client = 51003; //是否生成客户端代码
  bool disable_generic_services_server = 51004; //是否生成服务端代码
  bool generic_markdown_doc = 51005; //是否生成文档
  bool generic_objectfactory = 51006; //是否生成对象创建工厂

  bool generic_http_api_routes = 51007; //是否生成
}

//
message HttpApiOption {
    string path = 1 ; // 路径
    string method = 2 ; //请求的方法

    string description = 3 ;//注释说明
}

```

定义服务描述文件,这里实现一个Add的功能

```protobuf

syntax = "proto3";
package dotbpe;

option csharp_namespace = "MathCommon";

import public "dotbpe_option.proto";
option optimize_for = SPEED;

message AddReq {
  int32 a = 1 ;
  int32 b = 2 ;
}

message AddRes {
    int32 c = 1 ;
}


service Math{
    option (service_id)= 10005 ;//设定服务ID
    rpc Add (AddReq) returns (AddRes){
        option (message_id)= 1 ;//设定消息ID
    };//尾部的注释

}

```
2. 使用使用protoc 命令行+[protoc-gen-dotbpe](https://github.com/dotbpe/protoc-gen-dotbpe)插件来生成代码:

```shell
set -ex

cd $(dirname $0)


unameOut="$(uname -s)"
case "${unameOut}" in
    Linux*)     machine=Linux;;
    Darwin*)    machine=Mac;;
    CYGWIN*)    machine=Cygwin;;
    MINGW*)     machine=MinGw;;
    windows*)   machine=Windows;;
    *)          machine="UNKNOWN:${unameOut}"
esac


PROTOC=protoc

if [ $machine = "Windows" ] ; then
  PLUGIN=protoc-gen-dotbpe=dotbpe-amp-link.cmd
elif [ $machine = "Cygwin" ] ; then
  PLUGIN=protoc-gen-dotbpe=dotbpe-amp-link.cmd
elif [ $machine = "MinGw" ] ; then
  PLUGIN=protoc-gen-dotbpe=dotbpe-amp-link.cmd
else
  PLUGIN=protoc-gen-dotbpe=dotbpe-amp-link
fi


OUT_DIR=./MathCommon/_g

PROTO_DIR=./protos
BASE_PROTO_DIR=../../protos

if [ -d $OUT_DIR ]; then
  rm -rf $OUT_DIR
fi

mkdir -p $OUT_DIR


$PROTOC -I=$BASE_PROTO_DIR -I=$PROTO_DIR  --csharp_out=$OUT_DIR --dotbpe_out=$OUT_DIR \
	$PROTO_DIR/math.proto --plugin=$PLUGIN

```

代码生成到目录./MathCommon/_g


3. 编写服务端代码
实现对应的基类，并完善

```CSharp
public class MathService : MathBase
{
    public override Task<RpcResult<AddRes>> AddAsync(AddReq req){
        var res = new AddRes();
        res.C  = req.A + req.B ;
        return Task.FromResult(new RpcResult<AddRes>() { Data = res });
    }
}
```

4. 编写客户端代码

```CSharp
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
```

4. 运行结果
先运行右侧服务端，再运行客户端

![](http://ww1.sinaimg.cn/large/697065c1ly1fnsx75mv9qj20t80asabe.jpg)

[本示例的详细代码地址][8]

## Http Gateway示例

本示例用于编写一个基于AspNetCore的HttpGateWay，本示例使用Agent模式 ，即Rpc服务和Http服务挂载在一个进程中（不同端口）

1. 新建控制台程序
2. 添加必要的DotBPE引用 如下所示

```shell
$> dotnet add package DotBPE.Rpc.Hosting //挂载DotBPE的Host扩展
$> dotnet add package DotBPE.Protocol.Amp //使用默认的AMP协议
$> dotnet add package DotBPE.Plugin.AspNetGateway //使用默认的AMP协议
$> dotnet add package Microsoft.AspNetCore //添加AspNetCore的支持

```

3. 编写服务描述文件,这里定义两个接口

```protobuf


syntax = "proto3";
package dotbpe;

option csharp_namespace = "GatewayForAspNet";

import public "dotbpe_option.proto";

option optimize_for = SPEED;
option (generic_objectfactory) = true;
option (generic_http_api_routes) = true; //生成路由配置信息

message HelloReq {
  string name = 1 ;
}

message HelloRes {
    string greet_word = 1 ;
}


service Greeter{
    option (service_id)= 10006 ;//设定服务ID

    rpc SayHello (HelloReq) returns (HelloRes){
        option (message_id)= 1 ;//设定消息ID

        option (http_api_option) = {
            path : "/api/greeter/sayhello",
            method : "get",
            description : "接口1"
        };

    };//尾部的注释

    rpc SayHelloAgain (HelloReq) returns (HelloRes){
        option (message_id)= 2 ;//设定消息ID

        option (http_api_option) = {
            path : "/api/greeter/sayhelloagain",
            method : "get",
            description : "接口2"
        };

    };//尾部的注释

}


```

4. 生成代码

5. 实现对应的服务代码

```CSharp
 public class GreeterService : GreeterBase
    {
        public override Task<RpcResult<HelloRes>> SayHelloAgainAsync(HelloReq request)
        {
            var res = new RpcResult<HelloRes>();

            res.Data = new HelloRes()
            {
                GreetWord = $"Hello {request.Name} Again!"
            };

            return Task.FromResult(res);
        }

        public override Task<RpcResult<HelloRes>> SayHelloAsync(HelloReq request)
        {
            var res = new RpcResult<HelloRes>();
            res.Data = new HelloRes()
            {
                GreetWord = $"Hello {request.Name} !"
            };
            return Task.FromResult(res);
        }
    }
```

6. 实现ForwardService，这个实现， 大多是一样的，可根据实际情况，修改部分代码，如需添加Session的支持等。

```CSharp
    /// <summary>
    /// 转发服务，需要根据自己的协议 将HTTP请求的数据转码成对应的协议二进制
    /// 并将协议二进制数据转换成对应的 Http响应数据（一般是json）
    /// </summary>
    public class ForwardService : AbstractForwardService<AmpMessage>
    {

        static readonly JsonFormatter AmpJsonFormatter = new JsonFormatter(new JsonFormatter.Settings(true));



        readonly ILogger<ForwardService> Logger;
        private static AmpCallInvoker _invoker;
        private static object _lockObj = new object();

        public ForwardService(IRpcClient<AmpMessage> rpcClient,
           IOptions<HttpRouterOption> optionsAccessor,ILogger<ForwardService> logger) : base(rpcClient, optionsAccessor, logger)
        {
            this.Logger = logger;
        }

        /// <summary>
        ///  将收集的请求数据转换层协议消息
        /// </summary>
        /// <param name="reqData">请求数据，从body,form和queryString中获取</param>
        /// <returns></returns>
        protected override AmpMessage EncodeRequest(RequestData reqData)
        {
            ushort serviceId = (ushort)reqData.ServiceId;
            ushort messageId = (ushort)reqData.MessageId;
            AmpMessage message = AmpMessage.CreateRequestMessage(serviceId, messageId);


            IMessage reqTemp = ProtobufObjectFactory.GetRequestTemplate(serviceId, messageId);
            if (reqTemp == null)
            {
                Logger.LogError("serviceId={0},messageId={1}的消息不存在", serviceId, messageId);
                return null;
            }

            try
            {
                var descriptor = reqTemp.Descriptor;
                if (!string.IsNullOrEmpty(reqData.RawBody))
                {
                    reqTemp = descriptor.Parser.ParseJson(reqData.RawBody);
                }

                if (reqData.Data.Count > 0)
                {
                    foreach (var field in descriptor.Fields.InDeclarationOrder())
                    {
                        if (reqData.Data.ContainsKey(field.Name))
                        {
                            field.Accessor.SetValue(reqTemp, reqData.Data[field.Name]);
                        }
                        else if (reqData.Data.ContainsKey(field.JsonName))
                        {
                            field.Accessor.SetValue(reqTemp, reqData.Data[field.JsonName]);
                        }

                    }
                }


                message.Data = reqTemp.ToByteArray();

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "从HTTP请求中解析数据错误:" + ex.Message);
                message = null;
            }

            return message;

        }
        /// <summary>
        /// 返回协议调用者
        /// </summary>
        /// <param name="rpcClient"></param>
        /// <returns></returns>
        protected override CallInvoker<AmpMessage> GetProtocolCallInvoker(IRpcClient<AmpMessage> rpcClient)
        {
            if(_invoker != null)
            {
                return _invoker;
            }
            else
            {
                lock (_lockObj)
                {
                    if(_invoker == null)
                    {
                        _invoker = new AmpCallInvoker(rpcClient);
                    }
                    return _invoker;
                }
            }
        }

        /// <summary>
        /// 从消息中读取SessionId ，只有在配置了需要保持状态的网关上才会执行，并且只有在没有sessionId的时候才读取
        /// </summary>
        /// <param name="message">返回到客户端解析前的消息</param>
        /// <returns></returns>
        protected override string GetSessionIdFromMessage(AmpMessage message)
        {
            if (message.Code == 0)
            {

                if (message != null)
                {
                    var rspTemp = ProtobufObjectFactory.GetResponseTemplate(message.ServiceId, message.MessageId);
                    if (rspTemp == null)
                    {
                        return base.GetSessionIdFromMessage(message);
                    }

                    if (message.Data != null)
                    {
                        rspTemp.MergeFrom(message.Data);
                    }
                    //提取内部的return_code 字段
                    var field_sessionId = rspTemp.Descriptor.FindFieldByName(Constants.SEESIONID_FIELD_NAME);
                    if (field_sessionId != null)
                    {
                        var ObjV = field_sessionId.Accessor.GetValue(rspTemp);
                        if (ObjV != null)
                        {
                            return ObjV.ToString();
                        }
                    }

                }
            }
            return base.GetSessionIdFromMessage(message);
        }


        /// <summary>
        /// 将消息序列化成JSON，可以使用自己喜欢的序列化组件
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected override string MessageToJson(AmpMessage message)
        {
            if (message.Code == 0)
            {
                var return_code = 0;
                var return_message = "";
                string ret = "";
                if (message != null)
                {
                    var rspTemp = ProtobufObjectFactory.GetResponseTemplate(message.ServiceId, message.MessageId);
                    if (rspTemp == null)
                    {
                        return ret;
                    }

                    if (message.Data != null)
                    {
                        rspTemp.MergeFrom(message.Data);
                    }

                    //提取return_message
                    var field_msg = rspTemp.Descriptor.FindFieldByName("return_message");
                    if (field_msg != null)
                    {
                        var retObjV = field_msg.Accessor.GetValue(rspTemp);
                        if (retObjV != null)
                        {
                            return_message = retObjV.ToString();
                        }
                    }
                    ret = AmpJsonFormatter.Format(rspTemp);
                    //TODO:清理内部的return_message ;
                }

                return "{\"return_code\":" + return_code + ",\"return_message\":\"" + return_message + "\",data:" + ret + "}";
            }
            else
            {
                return "{\"return_code\":" + message.Code + ",\"return_message\":\"\"}";
            }


        }
    }
```

7. 编写Startup.cs

```CSharp
public class Startup
    {
        public Startup(IConfiguration config)
        {
            this.Configuration = config;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //路由配置,也可以使用配置文件哦
            //添加路由信息
            services.AddRoutes();

            // 自动转发服务
            services.AddSingleton<IForwardService, ForwardService>();

            //添加服务端支持
            services.AddDotBPE();
            //注册业务的服务
            services.AddServiceActors<AmpMessage>((actors) =>
            {
                actors.Add<GreeterService>();
            });

            //添加本地代理模式客户端
            services.AddAgentClient<AmpMessage>();

            //添加RPC服务
            services.AddSingleton<IHostedService, VirtualRpcHostService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //使用网关
            app.UseGateWay();
        }
    }
```

8. 启动服务

```CSharp
 class Program
    {
        static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
           WebHost.CreateDefaultBuilder(args)
                 .ConfigureAppConfiguration((hostContext, config) =>
                 {
                     config.AddJsonFile("dotbpe.json", optional: true, reloadOnChange: true)
                       .AddJsonFile($"dotbpe.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true)
                       .AddEnvironmentVariables(prefix: "DOTBPE_");
                 })
               .UseSetting(HostDefaultKey.HOSTADDRESS_KEY, "0.0.0.0:6201") //RPC服务绑定在6201端口
               .UseUrls("http://0.0.0.0:6200") //HTTP绑定在6200端口
               .UseStartup<Startup>()
               .Build();
    }
```
9. 启动服务，并访问http://localhost:6200/api/greeter/sayhello?name=dobpe 看看效果

[代码的完整示例地址](https://github.com/dotbpe/dotbpe/tree/master/src/sample/06-gateway-aspnet)


## 一个真实世界的例子，
一个问卷调查系统的后台管理系统，需要另开篇单独讲讲，思路和实现方式。
[代码的完整示例地址](https://github.com/dotbpe/dotbpe/tree/develop/src/sample/99-survey)


## 反馈

可以通过 [https://github.com/dotbpe/dotbpe/issues](https://github.com/dotbpe/dotbpe/issues) 反馈问题
另外我创建了一个QQ群：
![](http://ww1.sinaimg.cn/large/697065c1ly1fnsy1a8apgj206a082t8y.jpg)

  [1]: https://github.com/dotbpe/dotbpe-sample/blob/master/01-hello/%20hello%E7%A4%BA%E4%BE%8B
  [2]: https://github.com/dotbpe/dotbpe-sample/tree/master/02-math-json
  [3]: https://github.com/dotbpe/dotbpe-sample/tree/master/03-math-msgpack
  [4]: https://github.com/dotbpe/dotbpe-sample/tree/master/04-math-protobuf
  [5]: https://github.com/dotbpe/dotbpe/blob/master/src/protos/dotbpe_option.proto
  [6]: https://github.com/dotbpe/dotbpe/tree/master/src/tool/ampplugin
  [7]: https://github.com/xuanye/protobuf-gen-csharp/tree/master/dist
  [8]: https://github.com/dotbpe/dotbpe-sample/tree/master/05-generate-service
