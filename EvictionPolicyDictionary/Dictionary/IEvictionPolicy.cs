using System.Collections.Generic;

namespace EvictionPolicyDictionary.Dictionary
{
    public interface IEvictionPolicy<TKey, TValue>
    {
        bool ContainsKey(TKey key);

        void Add(TKey key, TValue value);

        void Add(KeyValuePair<TKey, TValue> item);

        bool Remove(TKey key);

        bool TryGetValue(TKey key, out TValue value);

        TValue this[TKey key] { get; set; }

        ICollection<TKey> Keys { get; }

        ICollection<TValue> Values { get; }

        IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator();

        void Clear();

        bool Contains(KeyValuePair<TKey, TValue> item);

        bool Remove(KeyValuePair<TKey, TValue> item);

        int Count { get; }
    }
}
