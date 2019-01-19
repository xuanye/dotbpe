简洁不简单的服务网关实现步骤
0： 获取路由配置，如果有更新则更新本地路由配置
1： 从consul 配置处获取所有服务，服务ID 对应的 地址，
2： 在本地打起所有rpc 链接，并维护这些链接 （循环刷新consul[两种方式 一种是轮询  一致是阻塞轮询 comet]  获取最新的服务地址，并新建链接 或者  移除链接）
3： 接收请求，根据路由确定serviceid和msgid， 通过serviceid，msgid ，找到对应发IRpcClient，并从Request中获取
