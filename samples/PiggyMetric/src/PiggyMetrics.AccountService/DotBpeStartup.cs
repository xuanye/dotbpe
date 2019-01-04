using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using Microsoft.Extensions.DependencyInjection;
using PiggyMetrics.AccountService.Impl;
using PiggyMetrics.AccountService.Repository;
using PiggyMetrics.Common;

namespace PiggyMetrics.AccountService
{
    public class DotBpeStartup : StartupBase
    {
        protected override void AddServiceActors(ActorsCollection<AmpMessage> actors)
        {
            actors.Add<AccountServiceImpl>();
        }

        protected override void AddBizServices(IServiceCollection services)
        {
            services.AddSingleton<AccountRepository>();
        }
    }
}
