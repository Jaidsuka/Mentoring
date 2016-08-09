using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EvictionPolicyDictionary.Dictionary
{
    public class ExpirableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly IEvictionPolicy<TKey, TValue> _evictionPolicy;
        protected Dictionary<TKey, TValue> Dictionary = new Dictionary<TKey, TValue>();

        public ExpirableDictionary(IEvictionPolicy<TKey, TValue> evictionPolicy)
        {
            _evictionPolicy = evictionPolicy;
            _evictionPolicy.Dictionary = _evictionPolicy.Dictionary ?? Dictionary;
        }

        public ExpirableDictionary(IEvictionPolicy<TKey, TValue> evictionPolicy, Dictionary<TKey, TValue> dictionary)
        {
            _evictionPolicy = evictionPolicy;
            _evictionPolicy.Dictionary = dictionary;
        }

        public bool ContainsKey(TKey key) => Keys.Any(x => x.Equals(key));

        public void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
            {
                throw new ArgumentException($"Key with {key} already exists");
            }

            _evictionPolicy.Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            if (ContainsKey(item.Key))
            {
                throw new ArgumentException($"Key with {item.Key} already exists");
            }

            _evictionPolicy.Add(item);
        }

        public bool Remove(TKey key) => _evictionPolicy.Remove(Dictionary.FirstOrDefault(x => x.Key.Equals(key)));

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (ContainsKey(key))
            {
                value = _evictionPolicy.GetValue(key);
                return true;
            }

            value = default(TValue);
            return false;
        }

        public TValue this[TKey key]
        {
            get { return _evictionPolicy[key]; }
            set { _evictionPolicy[key] = value; }
        }

        public ICollection<TKey> Keys => Dictionary.Select(x => x.Key).ToList();
        public ICollection<TValue> Values => Dictionary.Select(x => x.Value).ToList();

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _evictionPolicy.GetEnumerator();

        public void Clear() => _evictionPolicy.Clear();

        public bool Contains(KeyValuePair<TKey, TValue> item) => Dictionary.Any(x => x.Key.Equals(item.Key) && x.Value.Equals(item.Value));

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) => _evictionPolicy.Remove(item);

        public int Count => Dictionary.Count;
        public bool IsReadOnly => true;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
