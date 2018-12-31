using System;
using System.Collections.Generic;
using DotBPE.Rpc.Protocol;
using DotBPE.Rpc.Server;

namespace DotBPE.Rpc
{
    public class ServiceActorCollection
    {
        private readonly List<Type> _list;
        private readonly List<IServiceActor<AmpMessage>> _instances;

        public ServiceActorCollection()
        {
            _list = new List<Type>();
            _instances = new List<IServiceActor<AmpMessage>>();
        }

        public ServiceActorCollection Add<TActor>() where TActor : class, IServiceActor<AmpMessage>
        {
            if (!_list.Contains(typeof(TActor)))
            {
                _list.Add(typeof(TActor));
            }
            return this;
        }

        public ServiceActorCollection Add<TActor>(TActor actor) where TActor : class, IServiceActor<AmpMessage>
        {
            _instances.Add(actor);
            return this;
        }

        public List<Type> GetTypeAll()
        {
            return _list;
        }

        public List<IServiceActor<AmpMessage>> GetInstanceAll()
        {
            return _instances;
        }
    }
}
