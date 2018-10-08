using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc
{
    public interface IServiceDependencyRegistry
    {
        IServiceCollection AddServiceDependency(IServiceCollection services);
    }
}
