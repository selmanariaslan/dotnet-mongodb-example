using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arch.CoreLibrary.Utils.Variables
{
    public static class DecimalUtils
    {
        /// <summary>
        /// Rounds the supplied decimal to the specified amount of decimal points
        /// </summary>
        /// <param name="val">The decimal to round</param>
        /// <param name="decimalPoints">The number of decimal points to round the output value to</param>
        /// <returns>A rounded decimal</returns>
        public static decimal RoundDecimalPoints(this decimal val, int decimalPoints)
        {
            return Math.Round(val, decimalPoints);
        }

        /// <summary>
        /// Rounds the supplied decimal value to two decimal points
        /// </summary>
        /// <param name="val">The decimal to round</param>
        /// <returns>A decimal value rounded to two decimal points</returns>
        public static decimal RoundToTwoDecimalPoints(this decimal val)
        {
            return Math.Round(val, 2);
        }

        /// <summary>
        /// Returns the absolute value of a <see cref="System.Decimal"/> number
        /// </summary>
        /// <param name="this"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal Abs(this decimal value)
        {
            return Math.Abs(value);
        }

        /// <summary>Checks whether the value is in range</summary>
        /// <param name="value">The Value</param>
        /// <param name="minValue">The minimum value</param>
        /// <param name="maxValue">The maximum value</param>
        public static bool InRange(this decimal value, decimal minValue, decimal maxValue)
        {
            return value >= minValue && value <= maxValue;
        }

        /// <summary>Checks whether the value is in range or returns the default value</summary>
        /// <param name="value">The Value</param>
        /// <param name="minValue">The minimum value</param>
        /// <param name="maxValue">The maximum value</param>
        /// <param name="defaultValue">The default value</param>
        public static decimal InRange(this decimal value, decimal minValue, decimal maxValue, decimal defaultValue)
        {
            return value.InRange(minValue, maxValue) ? value : defaultValue;
        }

        /// <summary>
        /// Format a decimal using the local culture currency settings.
        /// </summary>
        /// <param name="value">The decimal to be formatted.</param>
        /// <returns>The decimal formatted based on the local culture currency settings.</returns>
        public static string ToLocalCurrencyString(this decimal value)
        {
            return String.Format("{0:C}", value);
        }

        /// <summary>
        /// Format a decimal using specific culture currency settings.
        /// </summary>
        /// <param name="value">The decimal to be formatted.</param>
        /// <param name="cultureName">The string representation for the culture to be used, for instance "en-US" for US English.</param>
        /// <returns>The decimal formatted based on specific culture currency settings.</returns>
        public static string ToSpecificCurrencyString(this decimal value, string cultureName)
        {
            CultureInfo currentCulture = new CultureInfo(cultureName);
            return string.Format(currentCulture, "{0:C}", value);
        }

        /// <summary>
        /// Abs
        /// </summary>
        /// <param name="this"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnumerable<decimal> Abs(this IEnumerable<decimal> value)
        {
            foreach (decimal d in value)
                yield return d.Abs();
        }

        public static Decimal ToDecimal(this string value)
        {
            Decimal result = 0M;

            if (!string.IsNullOrEmpty(value))
                Decimal.TryParse(value, out result);

            return result;
        }
    }
}
