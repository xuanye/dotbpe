# 在此处输入标题

标签（空格分隔）： 未分类

---


dotbpe ![](https://travis-ci.org/xuanye/dotbpe.svg?branch=master)
-------------
dotbpe是一款基于DOTNET Core编写的RPC框架，但是它的目标不仅仅只是解决rpc的问题，而是解决整个业务解决方案的问题，封装在常见的项目产品开发中的通用组件，让开发人员只专注于开发业务逻辑代码。底层通信默认实现基于DotNetty，可替换成其他Socket通信组件。dotbpe使用的默认协议名称叫Amp,编解码使用谷歌的Protobuf3,不过这些默认实现都是可以替换的。



```shell
$> dotnet add package DotBPE.Rpc //DotBPE的核心服务
$> dotnet add package DotBPE.Rpc.Hosting //挂载DotBPE的Host扩展
$> dotnet add package DotBPE.Rpc.Netty //使用Netty作为通讯框架
$> dotnet add package DotBPE.Protocol.Amp //使用默认的AMP协议
```

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




  [1]: https://github.com/dotbpe/dotbpe/blob/develop/src/sample/01-hello/%20hello%E7%A4%BA%E4%BE%8B
  [2]: https://github.com/dotbpe/dotbpe/tree/develop/src/sample/02-math-json
  [3]: https://github.com/dotbpe/dotbpe/tree/develop/src/sample/03-math-msgpack
  [4]: https://github.com/dotbpe/dotbpe/tree/develop/src/sample/04-math-protobuf
