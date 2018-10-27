using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc
{
    public interface IServiceDependencyRegistry
    {
        IServiceCollection AddServiceDependency(IConfiguration configuration, IServiceCollection services);
    }
}
