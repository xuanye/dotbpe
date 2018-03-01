using DotBPE.Rpc;
using DotBPE.Rpc.Server;

namespace DotBPE.Protocol.Amp
{
    public class ServiceActorLocator : AbstractServiceActorLocator<AmpMessage>
    {

        public ServiceActorLocator(IServiceActorContainer<AmpMessage> container):base(container)
        {
          
        }

        protected override IServiceActor<AmpMessage> LocateDefaultServiceActor(AmpMessage message)
        {
            return NotFoundServiceActor.Default;
        }
    }
}
