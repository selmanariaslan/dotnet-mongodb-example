using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Arch.CoreLibrary.Utils.Collections
{
    public static class CollectionUtils
    {
        public static bool AddUnique<T>(this ICollection<T> collection, T value)
        {
            var alreadyHas = collection.Contains(value);
            if (!alreadyHas)
            {
                collection.Add(value);
            }
            return alreadyHas;
        }

        public static int AddRangeUnique<T>(this ICollection<T> collection, IEnumerable<T> values)
        {
            var count = 0;
            foreach (var value in values)
            {
                if (collection.AddUnique(value))
                    count++;
            }
            return count;
        }

        public static void RemoveWhere<T>(this ICollection<T> collection, Predicate<T> predicate)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            var deleteList = collection.Where(child => predicate(child)).ToList();
            deleteList.ForEach(t => collection.Remove(t));
        }

        public static bool IsEmpty(this ICollection collection)
        {
            return collection == null || collection.Count == 0;
        }

        public static bool IsEmpty<T>(this ICollection<T> collection)
        {
            return collection == null || collection.Count == 0;
        }

        public static bool IsEmpty(this IList collection)
        {
            return collection == null || collection.Count == 0;
        }

        public static bool IsEmpty<T>(this IList<T> collection)
        {
            return collection == null || collection.Count == 0;
        }

        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> list, string sortExpression)
        {
            sortExpression += "";
            string[] parts = sortExpression.Split(' ');
            bool descending = false;
            string property = "";

            if (parts.Length > 0 && parts[0] != "")
            {
                property = parts[0];

                if (parts.Length > 1)
                {
                    descending = parts[1].IndexOf("esc", StringComparison.OrdinalIgnoreCase) >= 0;
                }

                PropertyInfo prop = typeof(T).GetProperty(property);

                if (prop == null)
                {
                    throw new System.Exception(String.Format("No property '{0}' in + {1}'", property, typeof(T).Name));
                }

                if (descending)
                    return list.OrderByDescending(x => prop.GetValue(x, null));
                else
                    return list.OrderBy(x => prop.GetValue(x, null));
            }

            return list;
        }

        public static void ForEach<T>(this IEnumerable<T> values, Action<T> action)
        {
            foreach (var value in values)
                action(value);
        }

        public static IEnumerable<T> IgnoreNulls<T>(this IEnumerable<T> target)
        {
            if (ReferenceEquals(target, null))
                yield break;

            foreach (var item in target.Where(item => !ReferenceEquals(item, null)))
                yield return item;
        }

        public static TItem MaxItem<TItem, TValue>(this IEnumerable<TItem> items, Func<TItem, TValue> selector, out TValue maxValue)
            where TItem : class
            where TValue : IComparable
        {
            TItem maxItem = null;
            maxValue = default(TValue);

            foreach (var item in items)
            {
                if (item == null)
                    continue;

                var itemValue = selector(item);

                if ((maxItem != null) && (itemValue.CompareTo(maxValue) <= 0))
                    continue;

                maxValue = itemValue;
                maxItem = item;
            }

            return maxItem;
        }

        public static TItem MaxItem<TItem, TValue>(this IEnumerable<TItem> items, Func<TItem, TValue> selector)
            where TItem : class
            where TValue : IComparable
        {
            return items.MaxItem(selector, out TValue maxValue);
        }

        public static TItem MinItem<TItem, TValue>(this IEnumerable<TItem> items, Func<TItem, TValue> selector, out TValue minValue)
            where TItem : class
            where TValue : IComparable
        {
            TItem minItem = null;
            minValue = default(TValue);

            foreach (var item in items)
            {
                if (item == null)
                    continue;
                var itemValue = selector(item);

                if ((minItem != null) && (itemValue.CompareTo(minValue) >= 0))
                    continue;
                minValue = itemValue;
                minItem = item;
            }

            return minItem;
        }

        public static TItem MinItem<TItem, TValue>(this IEnumerable<TItem> items, Func<TItem, TValue> selector)
            where TItem : class
            where TValue : IComparable
        {
            return items.MinItem(selector, out TValue minValue);
        }

        public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> source, Func<T, TKey> expression)
        {
            return source == null ? Enumerable.Empty<T>() : source.GroupBy(expression).Select(i => i.First());
        }

        public static IEnumerable<TT> Randomize<TT>(this IEnumerable<TT> target)
        {
            Random r = new Random();

            return target.OrderBy(_ => r.Next());
        }

        public static IEnumerable<T> RemoveWhere<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            if (source == null)
                yield break;

            foreach (T t in source)
            {
                if (!predicate(t))
                {
                    yield return t;
                }
            }
        }

        [Obsolete("Use RemoveWhere instead..")]
        public static IEnumerable<T> RemoveAll<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            if (source == null)
                return Enumerable.Empty<T>();

            var list = source.ToList();
            list.RemoveAll(predicate);
            return list;
        }

        public static string ToCSV<T>(this IEnumerable<T> source, char separator)
        {
            if (source == null)
                return string.Empty;

            var csv = new StringBuilder();
            source.ForEach(value => csv.AppendFormat("{0}{1}", value, separator));
            return csv.ToString(0, csv.Length - 1);
        }

        public static string ToCSV<T>(this IEnumerable<T> source)
        {
            return source.ToCSV(',');
        }

        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector, bool allowNull = true)
        {
            foreach (var item in source)
            {
                var select = selector(item);
                if (allowNull || !Equals(select, default(TSource)))
                    yield return select;
            }
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source?.Any() != true;
        }

        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return !source.IsNullOrEmpty();
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> source, T defaultValue)
        {
            return source.IsNotNullOrEmpty() ? source.First() : defaultValue;
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T item)
        {
            foreach (var i in source)
                yield return i;

            yield return item;
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, T item)
        {
            yield return item;

            foreach (var i in source)
                yield return i;
        }

        public static TResult[] ToArray<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            return source.Select(selector).ToArray();
        }

        public static List<TResult> ToList<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            return source.Select(selector).ToList();
        }

        public static uint Sum(this IEnumerable<uint> source)
        {
            return source.Aggregate(0U, (current, number) => current + number);
        }

        public static ulong Sum(this IEnumerable<ulong> source)
        {
            return source.Aggregate(0UL, (current, number) => current + number);
        }

        public static uint? Sum(this IEnumerable<uint?> source)
        {
            return source.Where(nullable => nullable.HasValue).Aggregate(0U, (current, nullable) => current + nullable.GetValueOrDefault());
        }

        public static ulong? Sum(this IEnumerable<ulong?> source)
        {
            return source.Where(nullable => nullable.HasValue).Aggregate(0UL, (current, nullable) => current + nullable.GetValueOrDefault());
        }

        public static uint Sum<T>(this IEnumerable<T> source, Func<T, uint> selection)
        {
            return ElementsNotNullFrom(source).Select(selection).Sum();
        }

        private static IEnumerable<T> ElementsNotNullFrom<T>(IEnumerable<T> source)
        {
            return source.Where(x => !EqualityComparer<T>.Default.Equals(x, default(T)));
        }

        public static uint? Sum<T>(this IEnumerable<T> source, Func<T, uint?> selection)
        {
            return ElementsNotNullFrom(source).Select(selection).Sum();
        }

        public static ulong Sum<T>(this IEnumerable<T> source, Func<T, ulong> selector)
        {
            return ElementsNotNullFrom(source).Select(selector).Sum();
        }

        public static ulong? Sum<T>(this IEnumerable<T> source, Func<T, ulong?> selector)
        {
            return ElementsNotNullFrom(source).Select(selector).Sum();
        }

        public static Dictionary<TKey, List<TValue>> ToDictionary<TKey, TValue>(this IEnumerable<IGrouping<TKey, TValue>> groupings)
        {
            return groupings.ToDictionary(group => group.Key, group => group.ToList());
        }

        public static bool HasCountOf<T>(this IEnumerable<T> source, int count)
        {
            return source.Take(count + 1).Count() == count;
        }

        public static IEnumerable<T> EnumValuesToList<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            Type enumType = typeof(T);

            if (enumType.BaseType != typeof(Enum))
                throw new ArgumentException("T must be of type System.Enum");

            Array enumValArray = Enum.GetValues(enumType);
            var enumValList = new List<T>(enumValArray.Length);
            enumValList.AddRange(from int val in enumValArray select (T)Enum.Parse(enumType, val.ToString()));

            return enumValList;
        }

        public static IEnumerable<string> EnumNamesToList<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            Type cls = typeof(T);

            Type[] enumArrayList = cls.GetInterfaces();

            return (from objType in enumArrayList where objType.IsEnum select objType.Name).ToList();
        }

        public static string ConcatWith<T>(this IEnumerable<T> items, string separator = ",", string formatString = "")
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (separator == null) throw new ArgumentNullException(nameof(separator));

            // shortcut for string enumerable
            if (typeof(T) == typeof(string))
            {
                return string.Join(separator, ((IEnumerable<string>)items).ToArray());
            }

            if (string.IsNullOrEmpty(formatString))
            {
                formatString = "{0}";
            }
            else
            {
                formatString = string.Format("{{0:{0}}}", formatString);
            }

            return string.Join(separator, items.Select(x => string.Format(formatString, x)).ToArray());
        }

        public static IEnumerable<T> Slice<T>(this IEnumerable<T> collection, int start, int end)
        {
            int index = 0;
            int count = 0;

            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            // Optimise item count for ICollection interfaces.
            if (collection is ICollection<T> t)
                count = t.Count;
            else if (collection is ICollection collections)
                count = collections.Count;
            else
                count = collection.Count();

            // Get start/end indexes, negative numbers start at the end of the collection.
            if (start < 0)
                start += count;

            if (end < 0)
                end += count;

            foreach (var item in collection)
            {
                if (index >= end)
                    yield break;

                if (index >= start)
                    yield return item;

                ++index;
            }
        }
    }
}
