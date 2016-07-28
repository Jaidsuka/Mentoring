using System;
using System.Collections.Generic;
using System.Linq;

namespace EvictionPolicyDictionary.Dictionary
{
    public class TimePeriodDiscardingDictionary<TKey, TValue> : BaseDictionary<TKey, TValue>
    {
        private readonly TimeSpan _period = TimeSpan.FromMinutes(30);
        private Dictionary<TKey, long> _usingTime = new Dictionary<TKey, long>();

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

        public override void Add(TKey key, TValue value)
        {
            TryCleanOutOfPeriodValues();
            if (ContainsKey(key))
            {
                throw new ArgumentException($"Key with {key} already exists");
            }

            AddWithTime(key, value);
        }

        public override void Add(KeyValuePair<TKey, TValue> item)
        {
            TryCleanOutOfPeriodValues();
            if (ContainsKey(item.Key))
            {
                throw new ArgumentException($"Key with {item.Key} already exists");
            }

            AddWithTime(item.Key, item.Value);
        }

        public override bool Remove(TKey key)
        {
            TryCleanOutOfPeriodValues();
            if (ContainsKey(key))
            {
                RemoveWithTime(key);
                return true;
            }

            return false;
        }

        public override bool TryGetValue(TKey key, out TValue value)
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

        public override TValue this[TKey key]
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
                    base[key] = value;
                }
                else
                {
                    AddWithTime(key, value);
                }
            }
        }

        public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var key in Keys)
            {
                var value = _usingTime[key];
                if (DateTime.Now.Ticks - value >= _period.Ticks)
                {
                    continue;
                }

                yield return new KeyValuePair<TKey, TValue>(key, base[key]);
            }
        }

        public override void Clear()
        {
            _usingTime = new Dictionary<TKey, long>();
            base.Clear();
        }

        public override bool Remove(KeyValuePair<TKey, TValue> item)
        {
            TryCleanOutOfPeriodValues();
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

        private void TryCleanOutOfPeriodValues()
        {
            foreach (var key in Keys)
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
            var value = base[key];
            return value;
        }
    }
}
