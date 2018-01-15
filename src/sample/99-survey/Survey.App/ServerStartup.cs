using DotBPE.Hosting;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using DotBPE.Rpc.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Survey.Service;
using Survey.Service.GateImpl;
using Survey.Service.InnerImpl;
using Survey.Service.InnerImpl.Repository;
using System;
using Vulcan.DataAccess;
using Vulcan.DataAccess.Context;

namespace Survey.App
{
    public static class ServerStartup
    { 

        /// <summary>
        /// 配置注入的服务信息
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            // 使用AMP协议
            services.AddDotBPE(); 

            //注册服务接收器
            AddServiceActors(services);

            //获取数据库连接
            services.Configure<DBOption>(context.Configuration.GetSection("connectionStrings"));

            //添加注入数据库的部分，当然不用注入，也可以直接new的方式，看自己喜欢了
            AddRepository(services);


            //设置上下文，可以在上下文中存储对象 在一次请求中可以传递
            services.AddSingleton<IContextAccessor<AmpMessage>, DefaultContextAccessor<AmpMessage>>();
            //Vulcan数据层需要的对象，如果不使用Vulcan可以不用注册
            services.AddSingleton<IRuntimeContextStorage, DotBPECallContextStorage<AmpMessage>>();
            //使用MySQL
            services.AddSingleton<IConnectionFactory, MySqlConnectionFactory>();

            //添加服务启动和关闭时需要注册的对象
            services.AddSingleton<IHostLifetime, ServiceBaseLifetime>();

            //添加挂载的宿主服务
            services.AddScoped<IHostedService, RpcHostedService>();

        }

       
        /// <summary>
        /// 注册其他需要注入的类
        /// </summary>
        /// <param name="services"></param>
        private static void AddRepository(IServiceCollection services)
        {
            //添加数据库
            services.AddSingleton<APaperRepository>()
                .AddSingleton<QPaperRepository>()
                .AddSingleton<UserRepository>();
        }

        /// <summary>
        /// 注册服务接收器
        /// </summary>
        /// <param name="services"></param>
        private static void AddServiceActors(IServiceCollection services)
        {

            // TODO:自动扫描所有服务
            // 添加本地加载的服务项
            services.AddServiceActors<AmpMessage>((actors) =>
            {
                //添加前置服务
                actors.Add<UserGateService>();
                actors.Add<SurveyGateService>();

                //添加内部服务
                actors.Add<UserInnerService>();
                actors.Add<QPaperInnerService>();
                actors.Add<APaperInnerService>();
            });
        }

    }
}
