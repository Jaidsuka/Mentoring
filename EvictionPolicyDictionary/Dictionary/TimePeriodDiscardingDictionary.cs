using System;
using System.Collections.Generic;
using System.Linq;

namespace EvictionPolicyDictionary.Dictionary
{
    public class TimePeriodDiscardingEvictionPolicy<TKey, TValue> : IEvictionPolicy<TKey, TValue>
    {
        private readonly TimeSpan _period = TimeSpan.FromMinutes(30);
        private Dictionary<TKey, long> _usingTime = new Dictionary<TKey, long>();

        public TimePeriodDiscardingEvictionPolicy() { }

        public TimePeriodDiscardingEvictionPolicy(TimeSpan period)
        {
            _period = period;
        }

        public Dictionary<TKey, TValue> Dictionary { get; set; }

        public void Add(TKey key, TValue value)
        {
            TryCleanOutOfPeriodValues();
            if (Dictionary.ContainsKey(key))
            {
                throw new ArgumentException($"Key with {key} already exists");
            }

            AddWithTime(key, value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            TryCleanOutOfPeriodValues();
            if (Dictionary.ContainsKey(item.Key))
            {
                throw new ArgumentException($"Key with {item.Key} already exists");
            }

            AddWithTime(item.Key, item.Value);
        }

        public bool Remove(TKey key)
        {
            TryCleanOutOfPeriodValues();
            if (Dictionary.ContainsKey(key))
            {
                RemoveWithTime(key);
                return true;
            }

            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            TryCleanOutOfPeriodValues();
            if (Dictionary.ContainsKey(key))
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
                TryCleanOutOfPeriodValues();
                if (Dictionary.ContainsKey(key))
                {
                    return GetValue(key); ;
                }

                throw new ArgumentException($"There is no value with this \"{key}\" value");
            }
            set
            {
                TryCleanOutOfPeriodValues();
                if (Dictionary.ContainsKey(key))
                {
                    _usingTime[key] = DateTime.Now.Ticks;
                    Dictionary[key] = value;
                }
                else
                {
                    AddWithTime(key, value);
                }
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var key in Dictionary.Keys)
            {
                var value = _usingTime[key];
                if (DateTime.Now.Ticks - value >= _period.Ticks)
                {
                    continue;
                }

                yield return new KeyValuePair<TKey, TValue>(key, Dictionary[key]);
            }
        }

        public void Clear()
        {
            _usingTime = new Dictionary<TKey, long>();
            Dictionary.Clear();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            TryCleanOutOfPeriodValues();
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

        private void TryCleanOutOfPeriodValues()
        {
            foreach (var key in Dictionary.Keys)
            {
                var value = _usingTime[key];
                if (DateTime.Now.Ticks - value >= _period.Ticks)
                {
                    RemoveWithTime(key);
                }
            }
        }
    }
}
