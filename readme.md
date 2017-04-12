dotbpe
-------------
dotbpe是一款基于CSharp编写的RPC框架，但是它的目标不仅仅只是解决rpc的问题，而是解决整个业务解决方案的问题，封装在常见的项目产品开发中的通用组件，让开发人员只专注于开发业务逻辑代码。底层通信默认实现基于DotNetty，可替换成其他Socket通信组件。dotbpe使用的默认协议名称叫Amp,编解码使用谷歌的Protobuf3,不过这些默认实现都是可以替换的。

## 待完成的功能
1. 服务端服务间互调逻辑封装
2. 客户端<-->服务端链接 心跳包
3. Protobuf代码生成插件
4. 日志和监控
5. 健康检查
6. 插件开发（HttpServer,HttpClient,RedisClient,DBClient,LocalCache）
7. 文档编写