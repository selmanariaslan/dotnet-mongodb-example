using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace ARCH.CoreLibrary.Utils.Collections
{

    public static class ListUtils
    {
        public static bool InsertUnique<T>(this IList<T> list, int index, T item)
        {
            if (!list.Contains(item))
            {
                list.Insert(index, item);
                return true;
            }
            return false;
        }

        public static int InsertRangeUnique<T>(this IList<T> list, int startIndex, IEnumerable<T> items)
        {
            var index = startIndex + items.Reverse().Count(item => list.InsertUnique(startIndex, item));
            return index - startIndex;
        }

        public static int IndexOf<T>(this IList<T> list, Func<T, bool> comparison)
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (comparison(list[i]))
                    return i;
            }
            return -1;
        }

        public static string Join<T>(this IList<T> list, char joinChar)
        {
            return list.Join(joinChar.ToString());
        }

        public static string Join<T>(this IList<T> list, string joinString)
        {
            if (list?.Any() != true)
                return string.Empty;

            StringBuilder result = new StringBuilder();

            int listCount = list.Count;
            int listCountMinusOne = listCount - 1;

            if (listCount > 1)
            {
                for (var i = 0; i < listCount; i++)
                {
                    if (i != listCountMinusOne)
                    {
                        result.Append(list[i]);
                        result.Append(joinString);
                    }
                    else
                    {
                        result.Append(list[i]);
                    }
                }
            }
            else
            {
                result.Append(list[0]);
            }

            return result.ToString();
        }

        public static List<T> Match<T>(this List<T> list, string searchString, int top, params Expression<Func<T, object>>[] args)
        {
            // Create a new list of results and matches;
            var results = new List<T>();
            var matches = new Dictionary<T, int>();
            var maxMatch = 0;
            // For each item in the source
            list.ForEach(s =>
            {
                // Generate the expression string from the argument.
                var regExp = string.Empty;
                if (args != null)
                {
                    // For each argument
                    Array.ForEach(args,
                        a =>
                        {
                            // Compile the expression
                            var property = a.Compile();
                            // Attach the new property to the expression string
                            regExp += String.Format("{0}{1})+?", string.IsNullOrEmpty(regExp) ? "(?:" : "|(?:", property(s));
                        });
                }
                // Get the matches
                var match = Regex.Matches(searchString, regExp, RegexOptions.IgnoreCase);
                // If there are more than one match
                if (match.Count > 0)
                {
                    // Add it to the match dictionary, including the match count.
                    matches.Add(s, match.Count);
                }
                // Get the highest max matching
                maxMatch = match.Count > maxMatch ? match.Count : maxMatch;
            });
            // Convert the match dictionary into a list
            var matchList = matches.ToList();

            // Sort the list by decending match counts
            // matchList.Sort((s1, s2) => s2.Value.CompareTo(s1.Value));

            // Remove all matches that is less than the best match.
            matchList.RemoveAll(s => s.Value < maxMatch);

            // If the top value is set and is less than the number of matches
            var getTop = top > 0 && top < matchList.Count ? top : matchList.Count;

            // Add the maches into the result list.
            for (var i = 0; i < getTop; i++)
                results.Add(matchList[i].Key);

            return results;
        }

        public static List<T> Cast<T>(this IList source)
        {
            var list = new List<T>();
            list.AddRange(source.OfType<T>());
            return list;
        }

        public static T GetRandomItem<T>(this IList<T> source, Random random)
        {
            if (source.Count > 0)
            {
                // The maxValue for the upper-bound in the Next() method is exclusive, see: http://stackoverflow.com/q/5063269/375958
                return source[random.Next(0, source.Count)];
            }
            else
            {
                throw new InvalidOperationException("Could not get item from empty list.");
            }
        }

        public static T GetRandomItem<T>(this IList<T> source, int seed)
        {
            var random = new Random(seed);
            return source.GetRandomItem(random);
        }

        public static T GetRandomItem<T>(this IList<T> source)
        {
            var random = new Random(System.DateTime.Now.Millisecond);
            return source.GetRandomItem(random);
        }

        #region Merge

        public static List<T> Merge<T>(params List<T>[] lists)
        {
            var merged = new List<T>();
            foreach (var list in lists) merged.Merge(list);
            return merged;
        }

        public static List<T> Merge<T>(Expression<Func<T, object>> match, params List<T>[] lists)
        {
            var merged = new List<T>();
            foreach (var list in lists) merged.Merge(list, match);
            return merged;
        }

        public static List<T> Merge<T>(this List<T> list1, List<T> list2, Expression<Func<T, object>> match)
        {
            if (list1 != null && list2 != null && match != null)
            {
                var matchFunc = match.Compile();
                foreach (var item in list2)
                {
                    var key = matchFunc(item);
                    if (!list1.Exists(i => matchFunc(i).Equals(key))) list1.Add(item);
                }
            }

            return list1;
        }

        public static List<T> Merge<T>(this List<T> list1, List<T> list2)
        {
            if (list1 != null && list2 != null) foreach (var item in list2.Where(item => !list1.Contains(item))) list1.Add(item);
            return list1;
        }

        #endregion Merge

        public static DataTable ConvertToDataTable<T>(this List<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));

            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }

                table.Rows.Add(row);
            }

            return table;

        }

        public static List<T> Shuffle<T>(this List<T> list)
        {
            var provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }
    }
}
