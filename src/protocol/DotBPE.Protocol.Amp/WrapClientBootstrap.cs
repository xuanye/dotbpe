using DotBPE.Rpc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Protocol.Amp
{
    public class WrapClientBootstrap : IClientBootstrap
    {
        private readonly IClientBootstrap<AmpMessage> _bootstrap;
        public WrapClientBootstrap(IClientBootstrap<AmpMessage> bootstrap)
        {
            _bootstrap = bootstrap;
        }
        public void Dispose()
        {
            _bootstrap.Dispose();
        }

        public Task StartAsync()
        {
            return _bootstrap.StartAsync();
        }

        public Task StopAsync()
        {
            return _bootstrap.StopAsync();
        }
    }
}
