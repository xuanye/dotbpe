using System;

namespace DotBPE.Rpc
{
    public interface IAppBuilder
    {
        IServiceProvider ServiceProvider{get;set;}

    }
}
