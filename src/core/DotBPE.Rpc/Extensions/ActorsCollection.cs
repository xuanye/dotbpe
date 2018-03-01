using DotBPE.Rpc.Codes;
using System;
using System.Collections.Generic;

namespace DotBPE.Rpc
{
    public class ActorsCollection<TMessage> where TMessage : InvokeMessage
    {
        private readonly List<Type> _list;
        private readonly List<IServiceActor<TMessage>> _instances;

        public ActorsCollection()
        {
            _list = new List<Type>();
            _instances = new List<IServiceActor<TMessage>>();
        }

        public ActorsCollection<TMessage> Add<TActor>() where TActor : class, IServiceActor<TMessage>
        {
            if (!_list.Contains(typeof(TActor)))
            {
                _list.Add(typeof(TActor));
            }
            return this;
        }

        public ActorsCollection<TMessage> Add<TActor>(TActor actor) where TActor : class, IServiceActor<TMessage>
        {
            _instances.Add(actor);
            return this;
        }

        public List<Type> GetTypeAll()
        {
            return _list;
        }

        public List<IServiceActor<TMessage>> GetInstanceAll()
        {
            return _instances;
        }
    }
}
