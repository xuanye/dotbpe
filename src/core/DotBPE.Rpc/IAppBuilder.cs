using System;
using DotBPE.Rpc.Codes;

namespace DotBPE.Rpc
{
    public interface IAppBuilder
    {
        IServiceProvider ServiceProvider{get;set;}

    }
}
