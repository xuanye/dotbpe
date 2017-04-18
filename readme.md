dotbpe
-------------
dotbpe是一款基于CSharp编写的RPC框架，但是它的目标不仅仅只是解决rpc的问题，而是解决整个业务解决方案的问题，封装在常见的项目产品开发中的通用组件，让开发人员只专注于开发业务逻辑代码。底层通信默认实现基于DotNetty，可替换成其他Socket通信组件。dotbpe使用的默认协议名称叫Amp,编解码使用谷歌的Protobuf3,不过这些默认实现都是可以替换的。

## 待完成的功能
1. 服务端服务间互调逻辑封装 ✓
2. 客户端<-->服务端链接 心跳包 ✓
3. Protobuf代码生成插件 ✓
4. 日志和监控
5. 健康检查
6. 插件开发（HttpServer,HttpClient,RedisClient,DBClient,LocalCache）
7. 文档编写
8. 客户端多链接实现




## 启动服务的过程

```
var host = new RpcHostBuilder()
            .UseConfiguration(config) //使用配置文件
            .AddRpcCore<AmpMessage>() //添加核心依赖
            .UseNettyServer<AmpMessage>()  //使用使用Netty默认实现
            .UseAmp() //使用Amp协议中的默认实现
            .UseAmpClient() //还要调用外部服务
            .AddServiceActor(new GreeterImpl())  //注册服务
            .Build();
```
1: 使用配置文件 UseConfiguration
2: 添加Rpc的核心组件 AddRpcCore
>IMessageHandler:
>负责服务端获得消息后交由它来处理，实际是在NettyChannel读取成功调用IMessageHandler的ReceiveAsync方法
>在默认的DefaultMessageHandler的ReceiveAsync 会触发接口的Recieved事件

>IServerHost
>仅仅是IServerBootstrap的包装

3: 添加NettyServerBootstrap的实现 UseNettyServer
4: 添加Amp协议的部分 UseAmp
>IMessageCodecs 编解码相关
>IServiceActorLocator 本地服务定位 通过ServiceId和MessageId组织一个特殊的标识去查找


5: 客户端实现（既是客户端又是服务端） UseAmpClient
>IRpcClient: 客户端实现，内部通过ITransportFactory 创建 ITransport，通过订阅IMessageHandler的Received事件来获取服务端返回
>ITransportFactory：缓存ITransport，通过地址EndPoint作为Key来查找或者重新创建
>ITransport: 对IClientBootstrap的包装,可在默认实现中添加多链接的实现
>IClientBootstrap：NettyClientBootstrap的实现

6:本地服务实现注册： AddServiceActor

