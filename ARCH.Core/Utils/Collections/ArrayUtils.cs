using ARCH.CoreLibrary.Utils.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARCH.CoreLibrary.Utils.Collections
{
    public static class ArrayUtils
    {
        public static bool IsNullOrEmpty(this Array source)
        {
            return source?.Length <= 0;
        }

        public static bool WithinIndex(this Array source, int index)
        {
            return source != null && index >= 0 && index < source.Length;
        }

        public static bool WithinIndex(this Array source, int index, int dimension = 0)
        {
            return source != null && index >= source.GetLowerBound(dimension) && index <= source.GetUpperBound(dimension);
        }

        public static T[] CombineArray<T>(this T[] combineWith, T[] arrayToCombine)
        {
            if (combineWith != default(T[]) && arrayToCombine != default(T[]))
            {
                int initialSize = combineWith.Length;
                Array.Resize<T>(ref combineWith, initialSize + arrayToCombine.Length);
                Array.Copy(arrayToCombine, arrayToCombine.GetLowerBound(0), combineWith, initialSize, arrayToCombine.Length);
            }
            return combineWith;
        }

        public static Array ClearAll(this Array clear)
        {
            if (clear != null)
                Array.Clear(clear, 0, clear.Length);
            return clear;
        }

        public static T[] ClearAll<T>(this T[] arrayToClear)
        {
            if (arrayToClear != null)
            {
                for (int i = arrayToClear.GetLowerBound(0); i <= arrayToClear.GetUpperBound(0); ++i)
                {
                    arrayToClear[i] = default(T);
                }
            }

            return arrayToClear;
        }

        public static Array ClearAt(this Array arrayToClear, int at)
        {
            if (arrayToClear != null)
            {
                int arrayIndex = at.GetArrayIndex();
                if (arrayIndex.IsIndexInArray(arrayToClear))
                    Array.Clear(arrayToClear, arrayIndex, 1);
            }
            return arrayToClear;
        }

        public static T[] ClearAt<T>(this T[] arrayToClear, int at)
        {
            if (arrayToClear != null)
            {
                int arrayIndex = at.GetArrayIndex();
                if (arrayIndex.IsIndexInArray(arrayToClear))
                    arrayToClear[arrayIndex] = default(T);
            }
            return arrayToClear;
        }

        public static bool IsEmpty(this Array array)
        {
            return array == null || array.Length == 0;
        }

        public static int FindArrayInArray(this byte[] buf1, byte[] buf2)
        {
            if (buf2 == null)
                throw new ArgumentNullException(nameof(buf2));

            if (buf1 == null)
                throw new ArgumentNullException(nameof(buf1));

            if (buf2.Length == 0)
                return 0;		// by definition empty sets match immediately

            int j = -1;
            int end = buf1.Length - buf2.Length;
            while ((j = Array.IndexOf(buf1, buf2[0], j + 1)) <= end && j != -1)
            {
                int i = 1;
                while (buf1[j + i] == buf2[i])
                {
                    if (++i == buf2.Length)
                        return j;
                }
            }
            return -1;
        }

        public static List<T> ToList<T>(this Array items, Func<object, T> mapFunction)
        {
            if (items == null || mapFunction == null)
                return new List<T>();

            List<T> coll = new List<T>();
            for (int i = 0; i < items.Length; i++)
            {
                T val = mapFunction(items.GetValue(i));
                if (!EqualityComparer<T>.Default.Equals(val, default(T)))
                    coll.Add(val);
            }
            return coll;
        }

        public static string ToString(this string[] values, string prefix = "(", string suffix = ")", string quotation = "\"", string separator = ",")
        {
            var sb = new StringBuilder();
            sb.Append(prefix);

            for (var i = 0; i < values.Length; i++)
            {
                if (i > 0)
                    sb.Append(separator);
                if (quotation != null)
                    sb.Append(quotation);
                sb.Append(values[i]);
                if (quotation != null)
                    sb.Append(quotation);
            }

            sb.Append(suffix);
            return sb.ToString();
        }

        #region BlockCopy

        public static T[] BlockCopy<T>(this T[] array, int index, int length)
        {
            return BlockCopy(array, index, length, false);
        }

        public static T[] BlockCopy<T>(this T[] array, int index, int length, bool padToLength)
        {
            if (array == null) throw new NullReferenceException();

            int n = length;
            T[] b = null;

            if (array.Length < index + length)
            {
                n = array.Length - index;
                if (padToLength)
                {
                    b = new T[length];
                }
            }

            if (b == null) b = new T[n];
            Array.Copy(array, index, b, 0, n);
            return b;
        }

        public static IEnumerable<T[]> BlockCopy<T>(this T[] array, int count, bool padToLength = false)
        {
            for (int i = 0; i < array.Length; i += count)
                yield return array.BlockCopy(i, count, padToLength);
        }

        #endregion
    }
}
