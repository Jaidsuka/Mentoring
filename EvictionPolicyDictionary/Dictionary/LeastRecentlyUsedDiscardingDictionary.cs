using System;
using System.Collections.Generic;
using System.Linq;

namespace EvictionPolicyDictionary.Dictionary
{
    public class LeastRecentlyUsedDiscardingDictionary<TKey, TValue> : BaseDictionary<TKey, TValue>
    {
        private readonly int _capacity = 5;
        private Dictionary<TKey, long> _usingTime = new Dictionary<TKey, long>();

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

        public override void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
            {
                throw new ArgumentException($"Key with {key} already exists");
            }

            TryCleanOutOfCapacityValue();
            AddWithTime(key, value);
        }

        public override void Add(KeyValuePair<TKey, TValue> item)
        {
            if (ContainsKey(item.Key))
            {
                throw new ArgumentException($"Key with {item.Key} already exists");
            }

            TryCleanOutOfCapacityValue();
            AddWithTime(item.Key, item.Value);
        }

        public override bool Remove(TKey key)
        {
            if (ContainsKey(key))
            {
                RemoveWithTime(key);
                return true;
            }

            return false;
        }

        public override bool TryGetValue(TKey key, out TValue value)
        {
            if (ContainsKey(key))
            {
                value = GetValue(key);
                return true;
            }

            value = default(TValue);
            return false;
        }

        public override TValue this[TKey key]
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
                    base[key] = value;
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        public override void Clear()
        {
            _usingTime = new Dictionary<TKey, long>();
            base.Clear();
        }

        public override bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (Contains(item))
            {
                var pair = Dictionary.FirstOrDefault(x => x.Key.Equals(item.Key) && x.Value.Equals(item));
                RemoveWithTime(pair.Key);
                return true;
            }

            return false;
        }

        private void AddWithTime(TKey key, TValue value)
        {
            base.Add(key, value);
            _usingTime.Add(key, DateTime.Now.Ticks);
        }

        private void RemoveWithTime(TKey key)
        {
            _usingTime.Remove(key);
            base.Remove(key);
        }

        private void TryCleanOutOfCapacityValue()
        {
            if (Keys.Count == _capacity)
            {
                var firstKey = Keys.First();
                KeyValuePair<TKey, long> pair = new KeyValuePair<TKey, long>(firstKey, _usingTime[firstKey]);
                foreach (var lapseKey in Keys)
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
            var value = base[key];
            return value;
        }
    }
}
