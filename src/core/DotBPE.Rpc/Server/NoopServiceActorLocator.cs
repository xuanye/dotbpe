namespace DotBPE.Rpc.Server
{
    public class NoopServiceActorLocator<TMessage> : IServiceActorLocator<TMessage> where TMessage : InvokeMessage
    {
        public IServiceActor<TMessage> LocateServiceActor(TMessage message)
        {
            return null;
        }
    }
}
