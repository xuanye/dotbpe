using Microsoft.Extensions.DependencyInjection;
using System;

namespace DotBPE.Rpc
{
    public interface IStartup
    {
        /// <summary>
        /// config services
        /// </summary>
        /// <param name="services"></param>
        IServiceProvider ConfigureServices(IServiceCollection services);

        /// <summary>
        /// config app build
        /// </summary>
        /// <param name="app">host builder</param>
        /// <param name="env"></param>
        /// <returns>return null will use default IServiceProvider</returns>
        void Configure(IAppBuilder app, IHostingEnvironment env);
    }
}
