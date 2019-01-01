# dotbpe

---


dotbpe ![](https://travis-ci.org/xuanye/dotbpe.svg?branch=master)
-------------

dotbpe一套基于dotnet core平台的业务流程处理引擎，力求解决项目开发中，关于服务端开发的各种通用问题，如远程过程调用（Rpc），延迟队列（DelayTaskQueue），任务调度（TaskManage），网关（Gateway）等问题。

dotbpe rpc 项目就是其中的Rpc部分的实现，底层的通讯部份基于[Peach](https://github.com/xuanye/peach)（基于DotNetty封装，支持定义协议的Socket类库）。该组件的目标并不是只是解决Rpc的问题，同时考虑到开发调式的便利性，支持本地服务和远端无差别开发，在编码时不用考虑服务是如何部署的（分布式或者单机部署），可以在项目初期流量较少时，只部署单台或者做简单的负载均衡即可，当项目流量增加后可通过配置和部署方案，不需要修改任何代码来实现快速扩容。

dotbpe rpc 支持两种开发模式，一种类似于Dubbo的定义接口的方式Rpc服务，另外一种支持像gRpc方式，预先定义服务描述文件（.proto）来定义Rpc服务.
> Note: 在同一个项目中应规范只使用一种，如无特殊情况，应避免交叉使用


**当前开发分支正在重构代码！**

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
    - 使用C#代码定义接口方式,类似Dubbo
    - 使用proto文件定义，类似gRpc （使用该方式则序列化只能使用Protobuf）
- 多种序列化方式（Protobuf、MessagePack、JSON）
- 支持多个扩展点
