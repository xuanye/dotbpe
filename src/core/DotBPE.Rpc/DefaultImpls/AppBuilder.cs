using System;
using DotBPE.Rpc.Codes;
using Microsoft.Extensions.DependencyInjection;

namespace DotBPE.Rpc.DefaultImpls
{
    public class AppBuilder : IAppBuilder
    {
        public IServiceProvider ServiceProvider { get;set; }

    }
}
