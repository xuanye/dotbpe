using DotBPE.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vulcan.DataAccess;
using Vulcan.DataAccess.Context;

namespace Survey.Service
{
    public class ServiceBaseLifetime : IHostLifetime
    {
        private IServiceProvider _hostProvider;

        public ServiceBaseLifetime(IServiceProvider hostProvider)
        {
            _hostProvider = hostProvider ?? throw new ArgumentNullException("hostProvider");
        }
        

        public void RegisterDelayStartCallback(Action<object> callback, object state)
        {
            //使用上下文存储数据库连接
            AppRuntimeContext.Configure(_hostProvider.GetRequiredService<IRuntimeContextStorage>());

            //设置使用哪种类型的数据库
            ConnectionFactoryHelper.Configure(_hostProvider.GetRequiredService<IConnectionFactory>());

            callback(state);
        }

        public void RegisterStopCallback(Action<object> callback, object state)
        {
            callback(state);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
