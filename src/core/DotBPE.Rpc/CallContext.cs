using System;
using System.Collections.Generic;
using DotBPE.Rpc.Codes;

namespace DotBPE.Rpc
{

    public class CallContext<TMessage>:IDisposable   where TMessage:InvokeMessage
    {
        private IRpcContext<TMessage> _context;
        private Dictionary<string,object> _items;

        public CallContext(IRpcContext<TMessage> context){
            this._context = context;
            _items= new Dictionary<string,object>();
        }

        public string RemoteAddress{
            get{
                return this._context.RemoteAddress;
            }
        }
        public string LocalAddress{
            get{
                return this._context.LocalAddress;
            }
        }


        public bool ContainsKey(string key)
        {
            return _items.ContainsKey(key);
        }

        public object Get(string key)
        {
            if(ContainsKey(key)){
                return _items[key];
            }
            return null;
        }

        public void Remove(string key)
        {
            if(ContainsKey(key)){
                _items.Remove(key);
            }

        }

        public void Set(string key,object item)
        {
            if(ContainsKey(key)){
                _items[key]=item;
            }
            else{
                _items.Add(key,item);
            }
        }

        public void Dispose()
        {
           this._context = null;
           this._items.Clear();
           this._items = null;
        }
    }
}
