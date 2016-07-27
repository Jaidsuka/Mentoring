using System;
using System.Collections.Generic;
using System.Linq;

namespace EvictionPolicyDictionary.Dictionary
{
    public class LeastRecentlyUsedDiscardingDictionary<TKey, TValue> : IEvictionPolicy<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>(); 
        private readonly Dictionary<TKey, long> _usingTime = new Dictionary<TKey, long>();
        private readonly int _capacity = 5;

        public LeastRecentlyUsedDiscardingDictionary() { }

        public LeastRecentlyUsedDiscardingDictionary(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentException($"{nameof(capacity)} can't be less than 1");
            }
            _capacity = capacity;
        }

        public LeastRecentlyUsedDiscardingDictionary(int capacity, IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentException("Dictionary can't be null");
            }

            if (dictionary.Keys.Count > capacity || capacity < 0)
            {
                throw new ArgumentException($"{nameof(capacity)} can't be less than count of dictionary keys and less than 1");
            }

            foreach (var key in dictionary.Keys)
            {
                AddWithTime(key, dictionary[key]);
            }
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.Keys.Any(x => x.Equals(key));
        }

        public void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
            {
                throw new ArgumentException($"Key with {key} already exists");
            }

            TryCleanOutOfCapacityValue();
            AddWithTime(key, value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            if (ContainsKey(item.Key))
            {
                throw new ArgumentException($"Key with {item.Key} already exists");
            }

            TryCleanOutOfCapacityValue();
            AddWithTime(item.Key, item.Value);
        }

        public bool Remove(TKey key)
        {
            if (ContainsKey(key))
            {
                RemoveWithTime(key);
                return true;
            }

            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (ContainsKey(key))
            {
                value = GetValue(key);
                return true;
            }

            value = default(TValue);
            return false;
        }

        public TValue this[TKey key]
        {
            get
            {
                if (ContainsKey(key))
                {
                    return GetValue(key); ;
                }

                throw new ArgumentException($"There is no value with this \"{key}\" value");
            }
            set
            {
                if (ContainsKey(key))
                {
                    _usingTime[key] = DateTime.Now.Ticks;
                    _dictionary[key] = value;
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        public ICollection<TKey> Keys => _dictionary.Keys;
        public ICollection<TValue> Values => _dictionary.Values;

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public void Clear()
        {
            foreach (var key in _dictionary.Keys)
            {
                RemoveWithTime(key);
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.Any(x => x.Key.Equals(item.Key) && x.Value.Equals(item.Value));
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (_dictionary.Any(x => x.Key.Equals(item.Key) && x.Value.Equals(item)))
            {
                var pair = _dictionary.FirstOrDefault(x => x.Key.Equals(item.Key) && x.Value.Equals(item));
                _dictionary.Remove(pair.Key);
                return true;
            }

            return false;
        }

        public int Count => _dictionary.Count;

        private void AddWithTime(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
            _usingTime.Add(key, DateTime.Now.Ticks);
        }

        private void RemoveWithTime(TKey key)
        {
            _usingTime.Remove(key);
            _dictionary.Remove(key);
        }

        private void TryCleanOutOfCapacityValue()
        {
            if (_dictionary.Keys.Count == _capacity)
            {
                var firstKey = _dictionary.Keys.First();
                KeyValuePair<TKey, long> pair = new KeyValuePair<TKey, long>(firstKey, _usingTime[firstKey]);
                foreach (var lapseKey in _dictionary.Keys)
                {
                    if (_usingTime[lapseKey] < pair.Value)
                    {
                        pair = new KeyValuePair<TKey, long>(lapseKey, _usingTime[lapseKey]);
                    }
                }

                RemoveWithTime(pair.Key);
            }
        }

        private TValue GetValue(TKey key)
        {
            _usingTime[key] = DateTime.Now.Ticks;
            var value = _dictionary[key];
            return value;
        }
    }
}
