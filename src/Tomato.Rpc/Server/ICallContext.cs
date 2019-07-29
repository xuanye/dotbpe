using System;

namespace Tomato.Rpc.Server
{
    public interface ICallContext:IDisposable
    {
        bool ContainsKey(string key);


        object Get(string key);


        void Remove(string key);

        void AddOrUpdate(string key, object item);

        void AddDef();
        void CloseDef();
    }
}
