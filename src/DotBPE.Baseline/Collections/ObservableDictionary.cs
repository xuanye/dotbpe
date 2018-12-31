using System;
using System.Collections;
using System.Collections.Generic;

namespace DotBPE.Baseline.Collections
{
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> _dictionary;

        public ObservableDictionary()
        {
            this._dictionary = new Dictionary<TKey, TValue>();
        }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            this._dictionary = new Dictionary<TKey, TValue>(dictionary);
        }

        public ObservableDictionary(IEqualityComparer<TKey> comparer)
        {
            this._dictionary = new Dictionary<TKey, TValue>(comparer);
        }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            this._dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
        }

        public void Add(TKey key, TValue value)
        {
            this._dictionary.Add(key, value);

            OnChanged(new ChangedEventArgs<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(key, value), ChangedAction.Add));
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            this._dictionary.Add(item);

            OnChanged(new ChangedEventArgs<KeyValuePair<TKey, TValue>>(item, ChangedAction.Add));
        }

        public bool Remove(TKey key)
        {
            bool success = this._dictionary.Remove(key);

            if (success)
                OnChanged(new ChangedEventArgs<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(key, default(TValue)), ChangedAction.Remove));

            return success;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            bool success = this._dictionary.Remove(item);

            if (success)
                OnChanged(new ChangedEventArgs<KeyValuePair<TKey, TValue>>(item, ChangedAction.Remove));

            return success;
        }

        public void Clear()
        {
            this._dictionary.Clear();

            OnChanged(new ChangedEventArgs<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(), ChangedAction.Clear));
        }

        public bool ContainsKey(TKey key)
        {
            return this._dictionary.ContainsKey(key);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return this._dictionary.Contains(item);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return this._dictionary.TryGetValue(key, out value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            this._dictionary.CopyTo(array, arrayIndex);
        }

        public ICollection<TKey> Keys
        {
            get { return this._dictionary.Keys; }
        }

        public ICollection<TValue> Values
        {
            get { return this._dictionary.Values; }
        }

        public int Count
        {
            get { return this._dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return this._dictionary.IsReadOnly; }
        }

        public TValue this[TKey key]
        {
            get { return this._dictionary[key]; }
            set
            {
                ChangedAction action = ContainsKey(key) ? ChangedAction.Update : ChangedAction.Add;

                this._dictionary[key] = value;
                OnChanged(new ChangedEventArgs<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(key, value), action));
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return this._dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public event EventHandler<ChangedEventArgs<KeyValuePair<TKey, TValue>>> Changed;

        private void OnChanged(ChangedEventArgs<KeyValuePair<TKey, TValue>> args)
        {
            if (Changed != null)
                Changed(this, args);
        }
    }

    public class ChangedEventArgs<T> : EventArgs
    {
        public T Item { get; private set; }
        public ChangedAction Action { get; private set; }

        public ChangedEventArgs(T item, ChangedAction action)
        {
            Item = item;
            Action = action;
        }
    }

    public enum ChangedAction
    {
        Add,
        Remove,
        Clear,
        Update
    }
}
