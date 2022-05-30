using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arch.CoreLibrary.Utils.Collections
{
    public static class DictionaryUtils
    {
        public static IDictionary<string, int> EnumToDictionary(this Type t)
        {
            if (t == null) throw new NullReferenceException();
            if (!t.IsEnum) throw new InvalidCastException("object is not an Enumeration");

            string[] names = Enum.GetNames(t);
            Array values = Enum.GetValues(t);

            return (from i in Enumerable.Range(0, names.Length)
                    select new { Key = names[i], Value = (int)values.GetValue(i) })
                        .ToDictionary(k => k.Key, k => k.Value);
        }

        public static IDictionary<TKey, TValue> Sort<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            return new SortedDictionary<TKey, TValue>(dictionary);
        }

        public static IDictionary<TKey, TValue> Sort<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IComparer<TKey> comparer)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            return new SortedDictionary<TKey, TValue>(dictionary, comparer);
        }

        public static IDictionary<TKey, TValue> SortByValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return (new SortedDictionary<TKey, TValue>(dictionary)).OrderBy(kvp => kvp.Value).ToDictionary(item => item.Key, item => item.Value);
        }

        public static IDictionary<TValue, TKey> Invert<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            return dictionary.ToDictionary(pair => pair.Value, pair => pair.Key);
        }

        public static Hashtable ToHashTable<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            var table = new Hashtable();

            foreach (var item in dictionary)
                table.Add(item.Key, item.Value);

            return table;
        }

        public static TValue GetFirstValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue defaultValue, params TKey[] keys)
        {
            foreach (var key in keys)
            {
                if (dictionary.ContainsKey(key))
                    return dictionary[key];
            }
            return defaultValue;
        }

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        {
            return source.GetOrDefault(key, default(TValue));
        }

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue defaultValue)
        {
            return source.TryGetValue(key, out TValue value) ? value : defaultValue;
        }

        public static TValue GetOrThrow<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, System.Exception exception)
        {
            if (source.TryGetValue(key, out TValue value))
            {
                return value;
            }

            throw exception;
        }

        public static bool IsEmpty(this IDictionary collection)
        {
            return collection == null || collection.Count == 0;
        }

        public static bool IsEmpty<TKey, TValue>(this IDictionary<TKey, TValue> collection)
        {
            return collection == null || collection.Count == 0;
        }
    }
}
