using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Core.Common
{
    public class CovariantReadOnlyDictionary<TKey, TValue, TReadOnlyValue> : IReadOnlyDictionary<TKey, TReadOnlyValue> where TValue : TReadOnlyValue
    {
        private readonly IDictionary<TKey, TValue> _dictionary;

        public CovariantReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        }
        public bool ContainsKey(TKey key) { return _dictionary.ContainsKey(key); }

        public IEnumerable<TKey> Keys => _dictionary.Keys;

        public bool TryGetValue(TKey key, out TReadOnlyValue value)
        {
            var result = _dictionary.TryGetValue(key, out var v);
            value = v;
            return result;
        }

        public IEnumerable<TReadOnlyValue> Values => _dictionary.Values.Cast<TReadOnlyValue>();

        public TReadOnlyValue this[TKey key] => _dictionary[key];

        public int Count => _dictionary.Count;

        public IEnumerator<KeyValuePair<TKey, TReadOnlyValue>> GetEnumerator()
        {
            return _dictionary
                .Select(x => new KeyValuePair<TKey, TReadOnlyValue>(x.Key, x.Value))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}