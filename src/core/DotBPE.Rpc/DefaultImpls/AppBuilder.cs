using System;

namespace DotBPE.Rpc.DefaultImpls
{
    public class AppBuilder : IAppBuilder
    {
        public IServiceProvider ServiceProvider { get; set; }
    }
}
