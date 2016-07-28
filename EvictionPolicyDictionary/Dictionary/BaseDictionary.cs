using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EvictionPolicyDictionary.Dictionary
{
    public abstract class BaseDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IEvictionPolicy<TKey, TValue>
    {
        protected List<KeyValuePair<TKey, TValue>> Dictionary = new List<KeyValuePair<TKey, TValue>>();

        public bool ContainsKey(TKey key)
        {
            return Keys.Any(x => x.Equals(key));
        }

        public virtual void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
            {
                throw new ArgumentException($"Key with {key} already exists");
            }

            Dictionary.Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public virtual void Add(KeyValuePair<TKey, TValue> item)
        {
            if (ContainsKey(item.Key))
            {
                throw new ArgumentException($"Key with {item.Key} already exists");
            }

            Dictionary.Add(item);
        }

        public virtual bool Remove(TKey key)
        {
            if (ContainsKey(key))
            {
                Dictionary.Remove(Dictionary.FirstOrDefault(x => x.Key.Equals(key)));
                return true;
            }

            return false;
        }

        public virtual bool TryGetValue(TKey key, out TValue value)
        {
            if (ContainsKey(key))
            {
                value = Dictionary.FirstOrDefault(x => x.Key.Equals(key)).Value;
                return true;
            }

            value = default(TValue);
            return false;
        }

        public virtual TValue this[TKey key]
        {
            get
            {
                if (ContainsKey(key))
                {
                    return Dictionary.FirstOrDefault(x => x.Key.Equals(key)).Value; ;
                }

                throw new ArgumentException($"There is no value with this \"{key}\" value");
            }
            set
            {
                if (ContainsKey(key))
                {
                    var item = Dictionary.FirstOrDefault(x => x.Key.Equals(key));
                    Dictionary.Remove(item);
                }

                Add(key, value);
            }
        }

        public ICollection<TKey> Keys => Dictionary.Select(x => x.Key).ToList();
        public ICollection<TValue> Values => Dictionary.Select(x => x.Value).ToList();

        public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }

        public virtual void Clear()
        {
            Dictionary = new List<KeyValuePair<TKey, TValue>>();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return Dictionary.Any(x => x.Key.Equals(item.Key) && x.Value.Equals(item.Value));
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
        }

        public virtual bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Dictionary.Remove(Dictionary.FirstOrDefault(x => x.Key.Equals(item.Key) && x.Value.Equals(item.Value)));
        }

        public int Count => Dictionary.Count;
        public bool IsReadOnly => true;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
