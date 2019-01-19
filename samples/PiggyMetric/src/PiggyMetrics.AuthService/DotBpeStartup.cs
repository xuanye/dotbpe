using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using Microsoft.Extensions.DependencyInjection;
using PiggyMetrics.AuthService.Impl;
using PiggyMetrics.Common;

namespace PiggyMetrics.AuthService
{
    public class DotBpeStartup : StartupBase
    {
        protected override void AddServiceActors(ActorsCollection<AmpMessage> actors)
        {
            actors.Add<AuthServiceImpl>();
        }

        protected override void AddBizServices(IServiceCollection services)
        {
           services.AddSingleton<Repository.AuthRepository>();
        }
    }
}
