using System.Collections.Generic;

namespace EvictionPolicyDictionary.Dictionary
{
    public interface IEvictionPolicy<TKey, TValue>
    {
        Dictionary<TKey, TValue> Dictionary { get; set; }

        void Add(TKey key, TValue value);

        void Add(KeyValuePair<TKey, TValue> item);

        bool Remove(TKey key);

        TValue GetValue(TKey key);

        TValue this[TKey key] { get; set; }

        IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator();

        void Clear();

        bool Remove(KeyValuePair<TKey, TValue> item);
    }
}
