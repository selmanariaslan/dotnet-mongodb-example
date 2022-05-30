using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arch.CoreLibrary.Utils.Variables
{
    public static class IntUtils
    {
        public static void Times(this int value, Action action)
        {
            value.Times(action);
        }

        public static void Times(this int value, Action<int> action)
        {
            // NOTE: Is it possible to reuse LongExtensions for this call?
            for (var i = 0; i < value; i++)
                action(i);
        }

        public static bool IsEven(this int value)
        {
            return value.IsEven();
        }

        public static bool IsOdd(this int value)
        {
            return value.IsOdd();
        }

        public static bool InRange(this int value, int minValue, int maxValue)
        {
            return value.InRange(minValue, maxValue);
        }

        public static int InRange(this int value, int minValue, int maxValue, int defaultValue)
        {
            return (int)value.InRange(minValue, maxValue, defaultValue);
        }

        public static bool IsPrime(this int candidate)
        {
            return candidate.IsPrime();
        }

        public static string ToOrdinal(this int i)
        {
            return i.ToOrdinal();
        }

        public static string ToOrdinal(this int i, string format)
        {
            return i.ToOrdinal(format);
        }

        public static long AsLong(this int i)
        {
            return i;
        }

        public static bool IsIndexInArray(this int index, Array arrayToCheck)
        {
            return index.GetArrayIndex().InRange(arrayToCheck.GetLowerBound(0), arrayToCheck.GetUpperBound(0));
        }

        public static int GetArrayIndex(this int at)
        {
            return at == 0 ? 0 : at - 1;
        }

        public static TimeSpan Days(this int days)
        {
            return TimeSpan.FromDays(days);
        }

        public static TimeSpan Hours(this int hours)
        {
            return TimeSpan.FromHours(hours);
        }

        public static TimeSpan Milliseconds(this int milliseconds)
        {
            return TimeSpan.FromMilliseconds(milliseconds);
        }

        public static TimeSpan Minutes(this int minutes)
        {
            return TimeSpan.FromMinutes(minutes);
        }

        public static TimeSpan Seconds(this int seconds)
        {
            return TimeSpan.FromSeconds(seconds);
        }

        public static TimeSpan Ticks(this int ticks)
        {
            return TimeSpan.FromTicks(ticks);
        }

        public static Int16 ToInt(this string value)
        {
            Int16 result = 0;

            if (!string.IsNullOrEmpty(value))
                Int16.TryParse(value, out result);

            return result;
        }
    }
}
