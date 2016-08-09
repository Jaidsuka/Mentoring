using System;
using System.Collections.Generic;
using System.Linq;

namespace EvictionPolicyDictionary.Dictionary
{
    public class LeastRecentlyUsedDiscardingEvictionPolicy<TKey, TValue> : IEvictionPolicy<TKey, TValue>
    {
        private readonly int _capacity = 5;
        private Dictionary<TKey, long> _usingTime = new Dictionary<TKey, long>();
        private Dictionary<TKey, TValue> _dictionary;

        public LeastRecentlyUsedDiscardingEvictionPolicy() { }

        public LeastRecentlyUsedDiscardingEvictionPolicy(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentException($"{nameof(capacity)} can't be less than 1");
            }

            _capacity = capacity;
        }

        public Dictionary<TKey, TValue> Dictionary
        {
            get { return _dictionary; }
            set
            {
                if (value?.Count > _capacity)
                {
                    throw new ArgumentException("You can't create LeastRecentlyUsedDiscardingEvictionPolicy with dictionary where count is higher than capacity.");
                }

                _dictionary = value;
            }
        }

        public void Add(TKey key, TValue value)
        {
            TryCleanOutOfCapacityValue();
            AddWithTime(key, value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            TryCleanOutOfCapacityValue();
            AddWithTime(item.Key, item.Value);
        }

        public bool Remove(TKey key)
        {
            if (Dictionary.ContainsKey(key))
            {
                RemoveWithTime(key);
                return true;
            }

            return false;
        }

        public TValue this[TKey key]
        {
            get
            {
                if (Dictionary.Any(x => x.Key.Equals(key)))
                {
                    return GetValue(key);
                }

                throw new ArgumentException($"There is no value with this \"{key}\" value");
            }
            set
            {
                if (Dictionary.Any(x => x.Key.Equals(key)))
                {
                    _usingTime[key] = DateTime.Now.Ticks;
                    Dictionary[key] = value;
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }

        public void Clear()
        {
            _usingTime = new Dictionary<TKey, long>();
            Dictionary.Clear();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (Dictionary.Contains(item))
            {
                var pair = Dictionary.FirstOrDefault(x => x.Key.Equals(item.Key) && x.Value.Equals(item));
                RemoveWithTime(pair.Key);
                return true;
            }

            return false;
        }

        public TValue GetValue(TKey key)
        {
            _usingTime[key] = DateTime.Now.Ticks;
            var value = Dictionary[key];
            return value;
        }

        private void AddWithTime(TKey key, TValue value)
        {
            Dictionary.Add(key, value);
            _usingTime.Add(key, DateTime.Now.Ticks);
        }

        private void RemoveWithTime(TKey key)
        {
            _usingTime.Remove(key);
            Dictionary.Remove(key);
        }

        private void TryCleanOutOfCapacityValue()
        {
            if (Dictionary.Count == _capacity)
            {
                var firstKey = Dictionary.Keys.First();
                KeyValuePair<TKey, long> pair = new KeyValuePair<TKey, long>(firstKey, _usingTime[firstKey]);
                foreach (var lapseKey in Dictionary.Keys)
                {
                    if (_usingTime[lapseKey] < pair.Value)
                    {
                        pair = new KeyValuePair<TKey, long>(lapseKey, _usingTime[lapseKey]);
                    }
                }

                RemoveWithTime(pair.Key);
            }
        }
    }
}
