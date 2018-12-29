# dotbpe

---


dotbpe ![](https://travis-ci.org/xuanye/dotbpe.svg?branch=master)
-------------
dotbpe是一款基于dotnet core（C#）编写的RPC框架，但是它的目标不仅仅只是解决rpc的问题，而是解决整个业务解决方案的问题，封装在常见的项目产品开发中的通用组件，让开发人员只专注于开发业务逻辑代码。底层通信默认实现基于DotNetty，可替换成其他Socket通信组件。dotbpe使用的默认协议名称叫Amp,编解码使用谷歌的Protobuf3,不过这些默认实现都是可以替换的。


当前开发分支正在重构代码！

## Features

- 高性能，易于使用的分布式远程调用框架
- 支持本地和远端服务自动调度
- 服务治理功能
    - 注册中心（可选）
    - 失败重试方式（可选）
    - 支持多种路由方式（可选）
- 支持依赖注入和AOP
- 支持服务分组
- Http网关
- 快速测试工具、
- 支持服务声明方式
    - 接口定义 Protobuf和MessagePack
    - proto文件定义
-  支持多个扩展点
