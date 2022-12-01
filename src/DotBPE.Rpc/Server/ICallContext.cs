// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Server
{
    public interface ICallContext : IDisposable
    {
        bool ContainsKey(string key);


        object Get(string key);


        void Remove(string key);

        void AddOrUpdate(string key, object item);


        void AddDef();
        void CloseDef();
    }
    public class CallContext : ICallContext
    {
        private readonly Dictionary<string, object> _items;

        private int _refCount;

        public CallContext()
        {
            _items = new Dictionary<string, object>();
        }


        public bool ContainsKey(string key)
        {
            return _items.ContainsKey(key);
        }

        public object Get(string key)
        {
            if (ContainsKey(key))
            {
                return _items[key];
            }
            return null;
        }

        public void Remove(string key)
        {
            if (ContainsKey(key))
            {
                _items.Remove(key);
            }
        }


        public void Dispose()
        {
            _items.Clear();
        }

        public void AddOrUpdate(string key, object item)
        {

            if (ContainsKey(key))
            {
                _items[key] = item;
            }
            else
            {
                _items.Add(key, item);
            }
        }

        public void AddDef()
        {
            _refCount++;
        }

        public void CloseDef()
        {
            _refCount--;
            if (_refCount <= 0)
            {
                Dispose();
            }
        }
    }
}
