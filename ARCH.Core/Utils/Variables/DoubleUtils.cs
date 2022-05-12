using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARCH.CoreLibrary.Utils.Variables
{
    public static class DoubleUtils
    {
        /// <summary>
        /// Rounds the supplied double to the specified amount of double points
        /// </summary>
        /// <param name="val">The double to round</param>
        /// <param name="doublePoints">The number of double points to round the output value to</param>
        /// <returns>A rounded double</returns>
        public static double RoundDoublePoints(this double val, int doublePoints)
        {
            return Math.Round(val, doublePoints);
        }

        /// <summary>
        /// Rounds the supplied double value to two double points
        /// </summary>
        /// <param name="val">The double to round</param>
        /// <returns>A double value rounded to two double points</returns>
        public static double RoundToTwoDoublePoints(this double val)
        {
            return Math.Round(val, 2);
        }

        /// <summary>
        /// Returns the absolute value of a <see cref="System.double"/> number
        /// </summary>
        /// <param name="this"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double Abs(this double value)
        {
            return Math.Abs(value);
        }

        /// <summary>Checks whether the value is in range</summary>
        /// <param name="value">The Value</param>
        /// <param name="minValue">The minimum value</param>
        /// <param name="maxValue">The maximum value</param>
        public static bool InRange(this double value, double minValue, double maxValue)
        {
            return value >= minValue && value <= maxValue;
        }

        /// <summary>Checks whether the value is in range or returns the default value</summary>
        /// <param name="value">The Value</param>
        /// <param name="minValue">The minimum value</param>
        /// <param name="maxValue">The maximum value</param>
        /// <param name="defaultValue">The default value</param>
        public static double InRange(this double value, double minValue, double maxValue, double defaultValue)
        {
            return value.InRange(minValue, maxValue) ? value : defaultValue;
        }

        /// <summary>
        /// Gets a TimeSpan from a double number of days.
        /// </summary>
        /// <param name="days">The number of days the TimeSpan will contain.</param>
        /// <returns>A TimeSpan containing the specified number of days.</returns>
        /// <remarks>
        ///		Contributed by jceddy
        /// </remarks>
        public static TimeSpan Days(this double days)
        {
            return TimeSpan.FromDays(days);
        }

        /// <summary>
        /// Gets a TimeSpan from a double number of hours.
        /// </summary>
        /// <param name="days">The number of hours the TimeSpan will contain.</param>
        /// <param name="hours"></param>
        /// <returns>A TimeSpan containing the specified number of hours.</returns>
        /// <remarks>
        ///		Contributed by jceddy
        /// </remarks>
        public static TimeSpan Hours(this double hours)
        {
            return TimeSpan.FromHours(hours);
        }

        /// <summary>
        /// Gets a TimeSpan from a double number of milliseconds.
        /// </summary>
        /// <param name="days">The number of milliseconds the TimeSpan will contain.</param>
        /// <param name="milliseconds"></param>
        /// <returns>A TimeSpan containing the specified number of milliseconds.</returns>
        /// <remarks>
        ///		Contributed by jceddy
        /// </remarks>
        public static TimeSpan Milliseconds(this double milliseconds)
        {
            return TimeSpan.FromMilliseconds(milliseconds);
        }

        /// <summary>
        /// Gets a TimeSpan from a double number of minutes.
        /// </summary>
        /// <param name="days">The number of minutes the TimeSpan will contain.</param>
        /// <param name="minutes"></param>
        /// <returns>A TimeSpan containing the specified number of minutes.</returns>
        /// <remarks>
        ///		Contributed by jceddy
        /// </remarks>
        public static TimeSpan Minutes(this double minutes)
        {
            return TimeSpan.FromMinutes(minutes);
        }

        /// <summary>
        /// Gets a TimeSpan from a double number of seconds.
        /// </summary>
        /// <param name="days">The number of seconds the TimeSpan will contain.</param>
        /// <param name="seconds"></param>
        /// <returns>A TimeSpan containing the specified number of seconds.</returns>
        /// <remarks>
        ///		Contributed by jceddy
        /// </remarks>
        public static TimeSpan Seconds(this double seconds)
        {
            return TimeSpan.FromSeconds(seconds);
        }

        /// <summary>
        /// Format a double using the local culture currency settings.
        /// </summary>
        /// <param name="value">The double to be formatted.</param>
        /// <returns>The double formatted based on the local culture currency settings.</returns>
        public static string ToLocalCurrencyString(this double value)
        {
            return string.Format("{0:C}", value);
        }

        /// <summary>
        /// Format a double using specific culture currency settings.
        /// </summary>
        /// <param name="value">The double to be formatted.</param>
        /// <param name="cultureName">The string representation for the culture to be used, for instance "en-US" for US English.</param>
        /// <returns>The double formatted based on specific culture currency settings.</returns>
        public static string ToSpecificCurrencyString(this double value, string cultureName)
        {
            CultureInfo currentCulture = new CultureInfo(cultureName);
            return string.Format(currentCulture, "{0:C}", value);
        }

        public static double ToDouble(this string value)
        {
            double result = 0;

            if (!string.IsNullOrEmpty(value))
                double.TryParse(value, out result);

            return result;
        }
    }
}
