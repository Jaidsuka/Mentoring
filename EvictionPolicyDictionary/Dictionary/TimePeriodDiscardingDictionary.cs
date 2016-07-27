using System;
using System.Collections.Generic;
using System.Linq;

namespace EvictionPolicyDictionary.Dictionary
{
    public class TimePeriodDiscardingDictionary<TKey, TValue> : IEvictionPolicy<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();
        private readonly Dictionary<TKey, long> _usingTime = new Dictionary<TKey, long>();
        private readonly TimeSpan _period = TimeSpan.FromMinutes(30);

        public TimePeriodDiscardingDictionary() { }

        public TimePeriodDiscardingDictionary(TimeSpan period)
        {
            _period = period;
        }

        public TimePeriodDiscardingDictionary(TimeSpan period, IDictionary<TKey, TValue> dictionary)
            : this(period)
        {
            if (dictionary == null)
            {
                throw new ArgumentException("Dictionary can't be null");
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
            TryCleanOutOfPeriodValues();
            if (ContainsKey(key))
            {
                throw new ArgumentException($"Key with {key} already exists");
            }

            AddWithTime(key, value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            TryCleanOutOfPeriodValues();
            if (ContainsKey(item.Key))
            {
                throw new ArgumentException($"Key with {item.Key} already exists");
            }

            AddWithTime(item.Key, item.Value);
        }

        public bool Remove(TKey key)
        {
            TryCleanOutOfPeriodValues();
            if (ContainsKey(key))
            {
                RemoveWithTime(key);
                return true;
            }

            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            TryCleanOutOfPeriodValues();
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
                TryCleanOutOfPeriodValues();
                if (ContainsKey(key))
                {
                    return GetValue(key); ;
                }

                throw new ArgumentException($"There is no value with this \"{key}\" value");
            }
            set
            {
                TryCleanOutOfPeriodValues();
                if (ContainsKey(key))
                {
                    _usingTime[key] = DateTime.Now.Ticks;
                    _dictionary[key] = value;
                }
                else
                {
                    AddWithTime(key, value);
                }
            }
        }

        public ICollection<TKey> Keys => _dictionary.Keys;
        public ICollection<TValue> Values => _dictionary.Values;

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var key in _dictionary.Keys)
            {
                var value = _usingTime[key];
                if (DateTime.Now.Ticks - value >= _period.Ticks)
                {
                    continue;
                }

                yield return new KeyValuePair<TKey, TValue>(key, _dictionary[key]);
            }
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
            TryCleanOutOfPeriodValues();
            if (_dictionary.Any(x => x.Key.Equals(item.Key) && x.Value.Equals(item)))
            {
                var pair = _dictionary.FirstOrDefault(x => x.Key.Equals(item.Key) && x.Value.Equals(item));
                RemoveWithTime(pair.Key);
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

        private void TryCleanOutOfPeriodValues()
        {
            foreach (var key in _dictionary.Keys)
            {
                var value = _usingTime[key];
                if (DateTime.Now.Ticks - value >= _period.Ticks)
                {
                    RemoveWithTime(key);
                }
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
