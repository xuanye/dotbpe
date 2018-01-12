using Microsoft.Extensions.DependencyInjection;
using System;

namespace DotBPE.Rpc.DefaultImpls
{
    public class DefaultStartup : IStartup
    {
        public virtual void Configure(IAppBuilder app, IHostingEnvironment env)
        {
        }

        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildServiceProvider();
        }
    }
}
