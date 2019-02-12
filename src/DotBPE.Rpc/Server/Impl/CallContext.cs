using System.Collections.Generic;

namespace DotBPE.Rpc.Server.Impl
{
    public class CallContext:ICallContext
    {
        private readonly Dictionary<string, object> _items;

        private int _refCount;

        public CallContext()
        {
            this._items = new Dictionary<string, object>();
        }


        public bool ContainsKey(string key)
        {
            return this._items.ContainsKey(key);
        }

        public object Get(string key)
        {
            if (ContainsKey(key))
            {
                return this._items[key];
            }
            return null;
        }

        public void Remove(string key)
        {
            if (ContainsKey(key))
            {
                this._items.Remove(key);
            }
        }


        public void Dispose()
        {
            this._items.Clear();
        }

        public void AddOrUpdate(string key, object item)
        {

            if (ContainsKey(key))
            {
                this._items[key] = item;
            }
            else
            {
                this._items.Add(key, item);
            }
        }

        public void AddDef()
        {
            this._refCount++;
        }

        public void CloseDef()
        {
            this._refCount--;
            if (this._refCount <= 0)
            {
                Dispose();
            }

        }
    }
}
