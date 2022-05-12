using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ARCH.CoreLibrary.Utils.Variables
{
    public static class StringUtils
    {
        #region Common string extensions

        /// <summary>
        /// 	Determines whether the specified string is null or empty.
        /// </summary>
        /// <param name = "value">The string value to check.</param>
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// 	Determines whether the specified string is not null or empty.
        /// </summary>
        /// <param name = "value">The string value to check.</param>
        public static bool IsNotEmpty(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// 	Checks whether the string is empty and returns a default value in case.
        /// </summary>
        /// <param name = "value">The string to check.</param>
        /// <param name = "defaultValue">The default value.</param>
        /// <returns>Either the string or the default value.</returns>
        public static string IfIsEmpty(this string value, string defaultValue)
        {
            return value.IsNotEmpty() ? value : defaultValue;
        }

        /// <summary>
        /// Throws an <see cref="System.ArgumentException"/> if the string value is empty.
        /// </summary>
        /// <param name="obj">The value to test.</param>
        /// <param name="message">The message to display if the value is null.</param>
        /// <param name="name">The name of the parameter being tested.</param>
        public static string ExceptionIfIsNullOrEmpty(this string obj, string message, string name)
        {
            if (String.IsNullOrEmpty(obj))
                throw new ArgumentException(message, name);
            return obj;
        }

        /// <summary>
        /// Joins  the values of a string array if the values are not null or empty.
        /// </summary>
        /// <param name="objs">The string array used for joining.</param>
        /// <param name="separator">The separator to use in the joined string.</param>
        /// <returns></returns>
        public static string JoinNotNullOrEmpty(this string[] objs, string separator)
        {
            var items = new List<string>();
            foreach (string s in objs)
            {
                if (!String.IsNullOrEmpty(s))
                    items.Add(s);
            }
            return String.Join(separator, items.ToArray());
        }

        /// <summary>
        /// 	Formats the value with the parameters using string.Format.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "parameters">The parameters.</param>
        /// <returns></returns>
        public static string FormatWith(this string value, params object[] parameters)
        {
            return string.Format(value, parameters);
        }

        /// <summary>
        /// 	Trims the text to a provided maximum length.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "maxLength">Maximum length.</param>
        /// <returns></returns>
        /// <remarks>
        /// 	Proposed by Rene Schulte
        /// </remarks>
        public static string TrimToMaxLength(this string value, int maxLength)
        {
            return value == null || value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        /// <summary>
        /// 	Trims the text to a provided maximum length and adds a suffix if required.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "maxLength">Maximum length.</param>
        /// <param name = "suffix">The suffix.</param>
        /// <returns></returns>
        /// <remarks>
        /// 	Proposed by Rene Schulte
        /// </remarks>
        public static string TrimToMaxLength(this string value, int maxLength, string suffix)
        {
            return value == null || value.Length <= maxLength ? value : string.Concat(value.Substring(0, maxLength), suffix);
        }

        /// <summary>
        /// 	Determines whether the comparison value strig is contained within the input value string
        /// </summary>
        /// <param name = "inputValue">The input value.</param>
        /// <param name = "comparisonValue">The comparison value.</param>
        /// <param name = "comparisonType">Type of the comparison to allow case sensitive or insensitive comparison.</param>
        /// <returns>
        /// 	<c>true</c> if input value contains the specified value, otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains(this string inputValue, string comparisonValue, StringComparison comparisonType)
        {
            return inputValue.IndexOf(comparisonValue, comparisonType) != -1;
        }

        /// <summary>
        /// 	Determines whether the comparison value string is contained within the input value string without any
        ///     consideration about the case (<see cref="StringComparison.InvariantCultureIgnoreCase"/>).
        /// </summary>
        /// <param name = "inputValue">The input value.</param>
        /// <param name = "comparisonValue">The comparison value.  Case insensitive</param>
        /// <returns>
        /// 	<c>true</c> if input value contains the specified value (case insensitive), otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsEquivalenceTo(this string inputValue, string comparisonValue)
        {
            return BothStringsAreEmpty(inputValue, comparisonValue) || StringContainsEquivalence(inputValue, comparisonValue);
        }

        /// <summary>
        /// Centers a charters in this string, padding in both, left and right, by specified Unicode character,
        /// for a specified total lenght.
        /// </summary>
        /// <param name="value">Instance value.</param>
        /// <param name="width">The number of characters in the resulting string, 
        /// equal to the number of original characters plus any additional padding characters.
        /// </param>
        /// <param name="padChar">A Unicode padding character.</param>
        /// <param name="truncate">Should get only the substring of specified width if string width is 
        /// more than the specified width.</param>
        /// <returns>A new string that is equivalent to this instance, 
        /// but center-aligned with as many paddingChar characters as needed to create a 
        /// length of width paramether.</returns>
        public static string PadBoth(this string value, int width, char padChar, bool truncate = false)
        {
            int diff = width - value.Length;
            if (diff == 0 || (diff < 0 && !truncate))
            {
                return value;
            }
            else if (diff < 0)
            {
                return value.Substring(0, width);
            }
            else
            {
                return value.PadLeft(width - (diff / 2), padChar).PadRight(width, padChar);
            }
        }

        /// <summary>
        /// 	Loads the string into a LINQ to XML XDocument
        /// </summary>
        /// <param name = "xml">The XML string.</param>
        /// <returns>The XML document object model (XDocument)</returns>
        public static XDocument ToXDocument(this string xml)
        {
            return XDocument.Parse(xml);
        }

        /// <summary>
        /// 	Loads the string into a XML DOM object (XmlDocument)
        /// </summary>
        /// <param name = "xml">The XML string.</param>
        /// <returns>The XML document object model (XmlDocument)</returns>
        public static XmlDocument ToXmlDom(this string xml)
        {
            var document = new XmlDocument();
            document.LoadXml(xml);
            return document;
        }

        /// <summary>
        /// 	Loads the string into a XML XPath DOM (XPathDocument)
        /// </summary>
        /// <param name = "xml">The XML string.</param>
        /// <returns>The XML XPath document object model (XPathNavigator)</returns>
        public static XPathNavigator ToXPath(this string xml)
        {
            var document = new XPathDocument(new StringReader(xml));
            return document.CreateNavigator();
        }

        /// <summary>
        ///     Loads the string into a LINQ to XML XElement
        /// </summary>
        /// <param name = "xml">The XML string.</param>
        /// <returns>The XML element object model (XElement)</returns>
        public static XElement ToXElement(this string xml)
        {
            return XElement.Parse(xml);
        }

        /// <summary>
        /// 	Reverses / mirrors a string.
        /// </summary>
        /// <param name = "value">The string to be reversed.</param>
        /// <returns>The reversed string</returns>
        public static string Reverse(this string value)
        {
            if (value.IsEmpty() || (value.Length == 1))
                return value;

            var chars = value.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        /// <summary>
        /// 	Ensures that a string starts with a given prefix.
        /// </summary>
        /// <param name = "value">The string value to check.</param>
        /// <param name = "prefix">The prefix value to check for.</param>
        /// <returns>The string value including the prefix</returns>
        /// <example>
        /// 	<code>
        /// 		var extension = "txt";
        /// 		var fileName = string.Concat(file.Name, extension.EnsureStartsWith("."));
        /// 	</code>
        /// </example>
        public static string EnsureStartsWith(this string value, string prefix)
        {
            return value.StartsWith(prefix) ? value : string.Concat(prefix, value);
        }

        /// <summary>
        /// 	Ensures that a string ends with a given suffix.
        /// </summary>
        /// <param name = "value">The string value to check.</param>
        /// <param name = "suffix">The suffix value to check for.</param>
        /// <returns>The string value including the suffix</returns>
        /// <example>
        /// 	<code>
        /// 		var url = "http://www.pgk.de";
        /// 		url = url.EnsureEndsWith("/"));
        /// 	</code>
        /// </example>
        public static string EnsureEndsWith(this string value, string suffix)
        {
            return value.EndsWith(suffix) ? value : string.Concat(value, suffix);
        }

        /// <summary>
        /// 	Repeats the specified string value as provided by the repeat count.
        /// </summary>
        /// <param name = "value">The original string.</param>
        /// <param name = "repeatCount">The repeat count.</param>
        /// <returns>The repeated string</returns>
        public static string Repeat(this string value, int repeatCount)
        {
            if (value.Length == 1)
                return new string(value[0], repeatCount);

            var sb = new StringBuilder(repeatCount * value.Length);
            while (repeatCount-- > 0)
                sb.Append(value);
            return sb.ToString();
        }

        /// <summary>
        /// 	Tests whether the contents of a string is a numeric value
        /// </summary>
        /// <param name = "value">String to check</param>
        /// <returns>
        /// 	Boolean indicating whether or not the string contents are numeric
        /// </returns>
        /// <remarks>
        /// 	Contributed by Kenneth Scott
        /// </remarks>
        public static bool IsNumeric(this string value)
        {
            return float.TryParse(value, out float output);
        }

        #region Extract

        /// <summary>
        /// 	Extracts all digits from a string.
        /// </summary>
        /// <param name = "value">String containing digits to extract</param>
        /// <returns>
        /// 	All digits contained within the input string
        /// </returns>
        /// <remarks>
        /// 	Contributed by Kenneth Scott
        /// </remarks>
        public static string ExtractDigits(this string value)
        {
            return value.Where(Char.IsDigit).Aggregate(new StringBuilder(value.Length), (sb, c) => sb.Append(c)).ToString();
        }

        #endregion

        /// <summary>
        /// 	Concatenates the specified string value with the passed additional strings.
        /// </summary>
        /// <param name = "value">The original value.</param>
        /// <param name = "values">The additional string values to be concatenated.</param>
        /// <returns>The concatenated string.</returns>
        public static string ConcatWith(this string value, params string[] values)
        {
            return string.Concat(value, string.Concat(values));
        }

        /// <summary>
        /// 	Convert the provided string to a Guid value.
        /// </summary>
        /// <param name = "value">The original string value.</param>
        /// <returns>The Guid</returns>
        public static Guid ToGuid(this string value)
        {
            return new Guid(value);
        }

        /// <summary>
        /// 	Convert the provided string to a Guid value and returns Guid.Empty if conversion fails.
        /// </summary>
        /// <param name = "value">The original string value.</param>
        /// <returns>The Guid</returns>
        public static Guid ToGuidSave(this string value)
        {
            return value.ToGuidSave(Guid.Empty);
        }

        /// <summary>
        /// 	Convert the provided string to a Guid value and returns the provided default value if the conversion fails.
        /// </summary>
        /// <param name = "value">The original string value.</param>
        /// <param name = "defaultValue">The default value.</param>
        /// <returns>The Guid</returns>
        public static Guid ToGuidSave(this string value, Guid defaultValue)
        {
            if (value.IsEmpty())
                return defaultValue;

            try
            {
                return value.ToGuid();
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 	Gets the string before the given string parameter.
        /// </summary>
        /// <param name = "value">The default value.</param>
        /// <param name = "x">The given string parameter.</param>
        /// <returns></returns>
        /// <remarks>Unlike GetBetween and GetAfter, this does not Trim the result.</remarks>
        public static string GetBefore(this string value, string x)
        {
            var xPos = value.IndexOf(x);
            return xPos == -1 ? String.Empty : value.Substring(0, xPos);
        }

        /// <summary>
        /// 	Gets the string between the given string parameters.
        /// </summary>
        /// <param name = "value">The source value.</param>
        /// <param name = "x">The left string sentinel.</param>
        /// <param name = "y">The right string sentinel</param>
        /// <returns></returns>
        /// <remarks>Unlike GetBefore, this method trims the result</remarks>
        public static string GetBetween(this string value, string x, string y)
        {
            var xPos = value.IndexOf(x);
            var yPos = value.LastIndexOf(y);

            if (xPos == -1 || xPos == -1)
                return String.Empty;

            var startIndex = xPos + x.Length;
            return startIndex >= yPos ? String.Empty : value.Substring(startIndex, yPos - startIndex).Trim();
        }

        /// <summary>
        /// 	Gets the string after the given string parameter.
        /// </summary>
        /// <param name = "value">The default value.</param>
        /// <param name = "x">The given string parameter.</param>
        /// <returns></returns>
        /// <remarks>Unlike GetBefore, this method trims the result</remarks>
        public static string GetAfter(this string value, string x)
        {
            var xPos = value.LastIndexOf(x);

            if (xPos == -1)
                return String.Empty;

            var startIndex = xPos + x.Length;
            return startIndex >= value.Length ? String.Empty : value.Substring(startIndex).Trim();
        }

        /// <summary>
        /// 	A generic version of System.String.Join()
        /// </summary>
        /// <typeparam name = "T">
        /// 	The type of the array to join
        /// </typeparam>
        /// <param name = "separator">
        /// 	The separator to appear between each element
        /// </param>
        /// <param name = "value">
        /// 	An array of values
        /// </param>
        /// <returns>
        /// 	The join.
        /// </returns>
        /// <remarks>
        /// 	Contributed by Michael T, http://about.me/MichaelTran
        /// </remarks>
        public static string Join<T>(string separator, T[] value)
        {
            if (value == null || value.Length == 0)
                return string.Empty;
            if (separator == null)
                separator = string.Empty;
            string converter(T o) => o.ToString();
            return string.Join(separator, Array.ConvertAll(value, converter));
        }

        /// <summary>
        /// 	Remove any instance of the given character from the current string.
        /// </summary>
        /// <param name = "value">
        /// 	The input.
        /// </param>
        /// <param name = "removeCharc">
        /// 	The remove char.
        /// </param>
        /// <remarks>
        /// 	Contributed by Michael T, http://about.me/MichaelTran
        /// </remarks>
        public static string Remove(this string value, params char[] removeCharc)
        {
            var result = value;
            if (!string.IsNullOrEmpty(result) && removeCharc != null)
                Array.ForEach(removeCharc, c => result = result.Remove(c.ToString()));

            return result;
        }

        /// <summary>
        /// Remove any instance of the given string pattern from the current string.
        /// </summary>
        /// <param name="value">The input.</param>
        /// <param name="strings">The strings.</param>
        /// <returns></returns>
        /// <remarks>
        /// Contributed by Michael T, http://about.me/MichaelTran
        /// </remarks>
        public static string Remove(this string value, params string[] strings)
        {
            return strings.Aggregate(value, (current, c) => current.Replace(c, string.Empty));
        }

        /// <summary>Finds out if the specified string contains null, empty or consists only of white-space characters</summary>
        /// <param name = "value">The input string</param>
        public static bool IsEmptyOrWhiteSpace(this string value)
        {
            return value.IsEmpty() || value.All(t => char.IsWhiteSpace(t));
        }

        /// <summary>Determines whether the specified string is not null, empty or consists only of white-space characters</summary>
        /// <param name = "value">The string value to check</param>
        public static bool IsNotEmptyOrWhiteSpace(this string value)
        {
            return !value.IsEmptyOrWhiteSpace();
        }

        /// <summary>Checks whether the string is null, empty or consists only of white-space characters and returns a default value in case</summary>
        /// <param name = "value">The string to check</param>
        /// <param name = "defaultValue">The default value</param>
        /// <returns>Either the string or the default value</returns>
        public static string IfIsEmptyOrWhiteSpace(this string value, string defaultValue)
        {
            return value.IsEmptyOrWhiteSpace() ? defaultValue : value;
        }

        /// <summary>Uppercase First Letter</summary>
        /// <param name = "value">The string value to process</param>
        public static string ToUpperFirstLetter(this string value)
        {
            if (value.IsEmptyOrWhiteSpace()) return string.Empty;

            char[] valueChars = value.ToCharArray();
            valueChars[0] = char.ToUpper(valueChars[0]);

            return new string(valueChars);
        }

        /// <summary>
        /// Returns the left part of the string.
        /// </summary>
        /// <param name="value">The original string.</param>
        /// <param name="characterCount">The character count to be returned.</param>
        /// <returns>The left part</returns>
        public static string Left(this string value, int characterCount)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (characterCount >= value.Length)
                throw new ArgumentOutOfRangeException(nameof(characterCount), characterCount, "characterCount must be less than length of string");
            return value.Substring(0, characterCount);
        }

        /// <summary>
        /// Returns the Right part of the string.
        /// </summary>
        /// <param name="value">The original string.</param>
        /// <param name="characterCount">The character count to be returned.</param>
        /// <returns>The right part</returns>
        public static string Right(this string value, int characterCount)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (characterCount >= value.Length)
                throw new ArgumentOutOfRangeException(nameof(characterCount), characterCount, "characterCount must be less than length of string");
            return value.Substring(value.Length - characterCount);
        }

        /// <summary>Returns the right part of the string from index.</summary>
        /// <param name="value">The original value.</param>
        /// <param name="index">The start index for substringing.</param>
        /// <returns>The right part.</returns>
        public static string SubstringFrom(this string value, int index)
        {
            return index < 0 ? value : value.Substring(index, value.Length - index);
        }

        //todo: xml documentation requires
        //todo: unit test required
        public static byte[] GetBytes(this string data)
        {
            return Encoding.Default.GetBytes(data);
        }

        public static byte[] GetBytes(this string data, Encoding encoding)
        {
            return encoding.GetBytes(data);
        }

        /// <summary>Convert text's case to a title case</summary>
        /// <param name="value"></param>
        /// <param name="culture"></param>
        /// <remarks>UppperCase characters is the source string after the first of each word are lowered, unless the word is exactly 2 characters</remarks>
        public static string ToTitleCase(this string value, CultureInfo culture)
        {
            return culture.TextInfo.ToTitleCase(value);
        }

        public static string ToPlural(this string singular)
        {
            // Multiple words in the form A of B : Apply the plural to the first word only (A)
            int index = singular.LastIndexOf(" of ");
            if (index > 0) return (singular.Substring(0, index)) + singular.Remove(0, index).ToPlural();

            // single Word rules
            //sibilant ending rule
            if (singular.EndsWith("sh")) return singular + "es";
            if (singular.EndsWith("ch")) return singular + "es";
            if (singular.EndsWith("us")) return singular + "es";
            if (singular.EndsWith("ss")) return singular + "es";
            //-ies rule
            if (singular.EndsWith("y")) return singular.Remove(singular.Length - 1, 1) + "ies";
            // -oes rule
            if (singular.EndsWith("o")) return singular.Remove(singular.Length - 1, 1) + "oes";
            // -s suffix rule
            return singular + "s";
        }

        /// <summary>
        /// Makes the current instance HTML safe.
        /// </summary>
        /// <param name="s">The current instance.</param>
        /// <returns>An HTML safe string.</returns>
        public static string ToHtmlSafe(this string s)
        {
            return s.ToHtmlSafe(false, false);
        }

        /// <summary>
        /// Makes the current instance HTML safe.
        /// </summary>
        /// <param name="s">The current instance.</param>
        /// <param name="all">Whether to make all characters entities or just those needed.</param>
        /// <returns>An HTML safe string.</returns>
        public static string ToHtmlSafe(this string s, bool all)
        {
            return s.ToHtmlSafe(all, false);
        }

        /// <summary>
        /// Makes the current instance HTML safe.
        /// </summary>
        /// <param name="s">The current instance.</param>
        /// <param name="all">Whether to make all characters entities or just those needed.</param>
        /// <param name="replace">Whether or not to encode spaces and line breaks.</param>
        /// <returns>An HTML safe string.</returns>
        public static string ToHtmlSafe(this string s, bool all, bool replace)
        {
            if (s.IsEmptyOrWhiteSpace())
                return string.Empty;
            var entities = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 28, 29, 30, 31, 34, 39, 38, 60, 62, 123, 124, 125, 126, 127, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 215, 247, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 8704, 8706, 8707, 8709, 8711, 8712, 8713, 8715, 8719, 8721, 8722, 8727, 8730, 8733, 8734, 8736, 8743, 8744, 8745, 8746, 8747, 8756, 8764, 8773, 8776, 8800, 8801, 8804, 8805, 8834, 8835, 8836, 8838, 8839, 8853, 8855, 8869, 8901, 913, 914, 915, 916, 917, 918, 919, 920, 921, 922, 923, 924, 925, 926, 927, 928, 929, 931, 932, 933, 934, 935, 936, 937, 945, 946, 947, 948, 949, 950, 951, 952, 953, 954, 955, 956, 957, 958, 959, 960, 961, 962, 963, 964, 965, 966, 967, 968, 969, 977, 978, 982, 338, 339, 352, 353, 376, 402, 710, 732, 8194, 8195, 8201, 8204, 8205, 8206, 8207, 8211, 8212, 8216, 8217, 8218, 8220, 8221, 8222, 8224, 8225, 8226, 8230, 8240, 8242, 8243, 8249, 8250, 8254, 8364, 8482, 8592, 8593, 8594, 8595, 8596, 8629, 8968, 8969, 8970, 8971, 9674, 9824, 9827, 9829, 9830 };
            var sb = new StringBuilder();
            foreach (var c in s)
            {
                if (all || entities.Contains(c))
                    sb.AppendFormat("&#{0};", (int)c);
                else
                    sb.Append(c);
            }

            return replace ? sb.Replace("", "<br />").Replace("\n", "<br />").Replace(" ", "&nbsp;").ToString() : sb.ToString();
        }

        /// <summary>
        /// Returns true if strings are equals, without consideration to case (<see cref="StringComparison.InvariantCultureIgnoreCase"/>)
        /// </summary>
        /// <param name="s"></param>
        /// <param name="whateverCaseString"></param>
        public static bool EquivalentTo(this string s, string whateverCaseString)
        {
            return string.Equals(s, whateverCaseString, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Replace all values in string
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="oldValues">List of old values, which must be replaced</param>
        /// <param name="replacePredicate">Function for replacement old values</param>
        /// <returns>Returns new string with the replaced values</returns>
        /// <example>
        /// 	<code>
        ///         var str = "White Red Blue Green Yellow Black Gray";
        ///         var achromaticColors = new[] {"White", "Black", "Gray"};
        ///         str = str.ReplaceAll(achromaticColors, v => "[" + v + "]");
        ///         // str == "[White] Red Blue Green Yellow [Black] [Gray]"
        /// 	</code>
        /// </example>
        /// <remarks>
        /// 	Contributed by nagits, http://about.me/AlekseyNagovitsyn
        /// </remarks>
        public static string ReplaceAll(this string value, IEnumerable<string> oldValues, Func<string, string> replacePredicate)
        {
            var sbStr = new StringBuilder(value);
            foreach (var oldValue in oldValues)
            {
                var newValue = replacePredicate(oldValue);
                sbStr.Replace(oldValue, newValue);
            }

            return sbStr.ToString();
        }

        /// <summary>
        /// Replace all values in string
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="oldValues">List of old values, which must be replaced</param>
        /// <param name="newValue">New value for all old values</param>
        /// <returns>Returns new string with the replaced values</returns>
        /// <example>
        /// 	<code>
        ///         var str = "White Red Blue Green Yellow Black Gray";
        ///         var achromaticColors = new[] {"White", "Black", "Gray"};
        ///         str = str.ReplaceAll(achromaticColors, "[AchromaticColor]");
        ///         // str == "[AchromaticColor] Red Blue Green Yellow [AchromaticColor] [AchromaticColor]"
        /// 	</code>
        /// </example>
        /// <remarks>
        /// 	Contributed by nagits, http://about.me/AlekseyNagovitsyn
        /// </remarks>
        public static string ReplaceAll(this string value, IEnumerable<string> oldValues, string newValue)
        {
            var sbStr = new StringBuilder(value);
            foreach (var oldValue in oldValues)
                sbStr.Replace(oldValue, newValue);

            return sbStr.ToString();
        }

        /// <summary>
        /// Replace all values in string
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="oldValues">List of old values, which must be replaced</param>
        /// <param name="newValues">List of new values</param>
        /// <returns>Returns new string with the replaced values</returns>
        /// <example>
        /// 	<code>
        ///         var str = "White Red Blue Green Yellow Black Gray";
        ///         var achromaticColors = new[] {"White", "Black", "Gray"};
        ///         var exquisiteColors = new[] {"FloralWhite", "Bistre", "DavyGrey"};
        ///         str = str.ReplaceAll(achromaticColors, exquisiteColors);
        ///         // str == "FloralWhite Red Blue Green Yellow Bistre DavyGrey"
        /// 	</code>
        /// </example>
        /// <remarks>
        /// 	Contributed by nagits, http://about.me/AlekseyNagovitsyn
        /// </remarks> 
        public static string ReplaceAll(this string value, IEnumerable<string> oldValues, IEnumerable<string> newValues)
        {
            var sbStr = new StringBuilder(value);
            var newValueEnum = newValues.GetEnumerator();
            foreach (var old in oldValues)
            {
                if (!newValueEnum.MoveNext())
                    throw new ArgumentOutOfRangeException(nameof(newValues), "newValues sequence is shorter than oldValues sequence");
                sbStr.Replace(old, newValueEnum.Current);
            }
            if (newValueEnum.MoveNext())
                throw new ArgumentOutOfRangeException(nameof(newValues), "newValues sequence is longer than oldValues sequence");

            return sbStr.ToString();
        }

        #endregion

        #region Bytes & Base64

        /// <summary>
        /// 	Converts the string to a byte-array using the default encoding
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <returns>The created byte array</returns>
        public static byte[] ToBytes(this string value)
        {
            return value.ToBytes(null);
        }

        /// <summary>
        /// 	Converts the string to a byte-array using the supplied encoding
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "encoding">The encoding to be used.</param>
        /// <returns>The created byte array</returns>
        /// <example>
        /// 	<code>
        /// 		var value = "Hello World";
        /// 		var ansiBytes = value.ToBytes(Encoding.GetEncoding(1252)); // 1252 = ANSI
        /// 		var utf8Bytes = value.ToBytes(Encoding.UTF8);
        /// 	</code>
        /// </example>
        public static byte[] ToBytes(this string value, Encoding encoding)
        {
            encoding = (encoding ?? Encoding.Default);
            return encoding.GetBytes(value);
        }

        /// <summary>
        /// 	Encodes the input value to a Base64 string using the default encoding.
        /// </summary>
        /// <param name = "value">The input value.</param>
        /// <returns>The Base 64 encoded string</returns>
        public static string EncodeBase64(this string value)
        {
            return value.EncodeBase64(null);
        }

        /// <summary>
        /// 	Encodes the input value to a Base64 string using the supplied encoding.
        /// </summary>
        /// <param name = "value">The input value.</param>
        /// <param name = "encoding">The encoding.</param>
        /// <returns>The Base 64 encoded string</returns>
        public static string EncodeBase64(this string value, Encoding encoding)
        {
            encoding = (encoding ?? Encoding.UTF8);
            var bytes = encoding.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 	Decodes a Base 64 encoded value to a string using the default encoding.
        /// </summary>
        /// <param name = "encodedValue">The Base 64 encoded value.</param>
        /// <returns>The decoded string</returns>
        public static string DecodeBase64(this string encodedValue)
        {
            return encodedValue.DecodeBase64(null);
        }

        /// <summary>
        /// 	Decodes a Base 64 encoded value to a string using the supplied encoding.
        /// </summary>
        /// <param name = "encodedValue">The Base 64 encoded value.</param>
        /// <param name = "encoding">The encoding.</param>
        /// <returns>The decoded string</returns>
        public static string DecodeBase64(this string encodedValue, Encoding encoding)
        {
            encoding = (encoding ?? Encoding.UTF8);
            var bytes = Convert.FromBase64String(encodedValue);
            return encoding.GetString(bytes);
        }

        #endregion

        #region String to Enum

        /// <summary>
        ///     Parse a string to a enum item if that string exists in the enum otherwise return the default enum item.
        /// </summary>
        /// <typeparam name="TEnum">The Enum type.</typeparam>
        /// <param name="dataToMatch">The data will use to convert into give enum</param>
        /// <returns>Converted enum.</returns>
        /// <example>
        /// 	<code>
        /// 		public enum EnumTwo {  None, One,}
        /// 		object[] items = new object[] { "One".ParseStringToEnum<EnumTwo>(), "Two".ParseStringToEnum<EnumTwo>() };
        /// 	</code>
        /// </example>
        /// <remarks>
        /// 	Contributed by Mohammad Rahman, http://mohammad-rahman.blogspot.com/
        /// </remarks>
        public static TEnum ParseStringToEnum<TEnum>(this string dataToMatch)
                where TEnum : struct
        {
            return dataToMatch.IsItemInEnum<TEnum>()() ? default(TEnum) : (TEnum)Enum.Parse(typeof(TEnum), dataToMatch, default(bool));
        }

        /// <summary>
        ///     To check whether the data is defined in the given enum.
        /// </summary>
        /// <typeparam name="TEnum">The enum will use to check, the data defined.</typeparam>
        /// <param name="dataToCheck">To match against enum.</param>
        /// <returns>Anonoymous method for the condition.</returns>
        /// <remarks>
        /// 	Contributed by Mohammad Rahman, http://mohammad-rahman.blogspot.com/
        /// </remarks>
        public static Func<bool> IsItemInEnum<TEnum>(this string dataToCheck)
            where TEnum : struct
        {
            return () => string.IsNullOrEmpty(dataToCheck) || !Enum.IsDefined(typeof(TEnum), dataToCheck);
        }

        #endregion

        private static bool StringContainsEquivalence(string inputValue, string comparisonValue)
        {
            return inputValue.IsNotEmptyOrWhiteSpace() && inputValue.Contains(comparisonValue, StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool BothStringsAreEmpty(string inputValue, string comparisonValue)
        {
            return inputValue.IsEmptyOrWhiteSpace() && comparisonValue.IsEmptyOrWhiteSpace();
        }

        /// <summary>
        /// Return the string with the leftmost number_of_characters characters removed.
        /// </summary>
        /// <param name="str">The string being extended</param>
        /// <param name="numberOfCharacters">The number of characters to remove.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static String RemoveLeft(this String str, int numberOfCharacters)
        {
            if (str.Length <= numberOfCharacters) return "";
            return str.Substring(numberOfCharacters);
        }

        /// <summary>
        /// Return the string with the rightmost number_of_characters characters removed.
        /// </summary>
        /// <param name="str">The string being extended</param>
        /// <param name="numberOfCharacters">The number of characters to remove.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static String RemoveRight(this String str, int numberOfCharacters)
        {
            if (str.Length <= numberOfCharacters) return "";
            return str.Substring(0, str.Length - numberOfCharacters);
        }

        /// <summary>
        /// Determines whether the string contains any of the provided values.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool ContainsAny(this string @this, params string[] values)
        {
            return @this.ContainsAny(StringComparison.CurrentCulture, values);
        }

        /// <summary>
        /// Determines whether the string contains any of the provided values.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="comparisonType"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool ContainsAny(this string @this, StringComparison comparisonType, params string[] values)
        {
            foreach (string value in values)
            {
                if (@this.IndexOf(value, comparisonType) > -1) return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether the string contains all of the provided values.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool ContainsAll(this string @this, params string[] values)
        {
            return @this.ContainsAll(StringComparison.CurrentCulture, values);
        }

        /// <summary>
        /// Determines whether the string contains all of the provided values.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="comparisonType"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool ContainsAll(this string @this, StringComparison comparisonType, params string[] values)
        {
            foreach (string value in values)
            {
                if (@this.IndexOf(value, comparisonType) == -1) return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether the string is equal to any of the provided values.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="comparisonType"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool EqualsAny(this string @this, StringComparison comparisonType, params string[] values)
        {
            foreach (string value in values)
            {
                if (@this.Equals(value, comparisonType)) return true;
            }
            return false;
        }

        /// <summary>
        /// Wildcard comparison for any pattern
        /// </summary>
        /// <param name="value">The current <see cref="System.String"/> object</param>
        /// <param name="patterns">The array of string patterns</param>
        /// <returns></returns>
        public static bool IsLikeAny(this string value, params string[] patterns)
        {
            foreach (string pattern in patterns)
            {
                if (value.IsLike(pattern)) return true;
            }
            return false;
        }

        /// <summary>
        /// Wildcard comparison
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static bool IsLike(this string value, string pattern)
        {
            if (value == pattern) return true;

            if (pattern[0] == '*' && pattern.Length > 1)
            {
                for (int index = 0; index < value.Length; index++)
                {
                    if (value.Substring(index).IsLike(pattern.Substring(1)))
                        return true;
                }
            }
            else if (pattern[0] == '*')
            {
                return true;
            }
            else if (pattern[0] == value[0])
            {
                return value.Substring(1).IsLike(pattern.Substring(1));
            }
            return false;
        }

        /// <summary>
        /// Truncates a string with optional Elipses added
        /// </summary>
        /// <param name="this"></param>
        /// <param name="length"></param>
        /// <param name="useElipses"></param>
        /// <returns></returns>
        public static string Truncate(this string @this, int length, bool useElipses = false)
        {
            int e = useElipses ? 3 : 0;
            if (length - e <= 0) throw new InvalidOperationException(string.Format("Length must be greater than {0}.", e));

            if (string.IsNullOrEmpty(@this) || @this.Length <= length) return @this;

            return @this.Substring(0, length - e) + new String('.', e);
        }

        /*
        var s = "aaaaaaaabbbbccccddddeeeeeeeeeeee".FormatWithMask("Hello ########-#A###-####-####-############ Oww");
            s.ShouldEqual("Hello aaaaaaaa-bAbbb-cccc-dddd-eeeeeeeeeeee Oww");
 
        var s = "abc".FormatWithMask("###-#");
                    s.ShouldEqual("abc-");
 
        var s = "".FormatWithMask("Hello ########-#A###-####-####-############ Oww");
                    s.ShouldEqual("");
         */

        /// <summary>
        /// Formats the string according to the specified mask
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="value"></param>
        /// <param name="mask">The mask for formatting. Like "A##-##-T-###Z"</param>
        /// <returns>The formatted string</returns>
        public static string FormatWithMask(this string value, string mask)
        {
            if (value?.Length == 0 || value == null) return value;
            var output = string.Empty;
            var index = 0;
            foreach (var m in mask)
            {
                if (m == '#')
                {
                    if (index < value.Length)
                    {
                        output += value[index];
                        index++;
                    }
                }
                else
                {
                    output += m;
                }
            }
            return output;
        }

        /// <summary>
        /// Mask the source string with the mask char except for the last exposed digits.
        /// </summary>
        /// <param name="sourceValue">Original string to mask.</param>
        /// <param name="maskChar">The character to use to mask the source.</param>
        /// <param name="numExposed">Number of characters exposed in masked value.</param>
        /// <param name="style">The masking style to use (all characters or just alpha-nums).</param>
        /// <returns>The masked account number.</returns>
        public static string Mask(this string sourceValue, char maskChar, int numExposed, MaskStyle style)
        {
            var maskedString = sourceValue;

            if (sourceValue.IsLengthAtLeast(numExposed))
            {
                var builder = new StringBuilder(sourceValue.Length);
                int index = maskedString.Length - numExposed;

                if (style == MaskStyle.AlphaNumericOnly)
                {
                    CreateAlphaNumMask(builder, sourceValue, maskChar, index);
                }
                else
                {
                    builder.Append(maskChar, index);
                }

                builder.Append(sourceValue.Substring(index));
                maskedString = builder.ToString();
            }

            return maskedString;
        }

        /// <summary>
        /// Mask the source string with the mask char except for the last exposed digits.
        /// </summary>
        /// <param name="sourceValue">Original string to mask.</param>
        /// <param name="maskChar">The character to use to mask the source.</param>
        /// <param name="numExposed">Number of characters exposed in masked value.</param>
        /// <returns>The masked account number.</returns>
        public static string Mask(this string sourceValue, char maskChar, int numExposed)
        {
            return Mask(sourceValue, maskChar, numExposed, MaskStyle.All);
        }

        /// <summary>
        /// Mask the source string with the mask char.
        /// </summary>
        /// <param name="sourceValue">Original string to mask.</param>
        /// <param name="maskChar">The character to use to mask the source.</param>
        /// <returns>The masked account number.</returns>
        public static string Mask(this string sourceValue, char maskChar)
        {
            return Mask(sourceValue, maskChar, 0, MaskStyle.All);
        }

        /// <summary>
        /// Mask the source string with the default mask char except for the last exposed digits.
        /// </summary>
        /// <param name="sourceValue">Original string to mask.</param>
        /// <param name="numExposed">Number of characters exposed in masked value.</param>
        /// <returns>The masked account number.</returns>
        public static string Mask(this string sourceValue, int numExposed)
        {
            return Mask(sourceValue, DefaultMaskCharacter, numExposed, MaskStyle.All);
        }

        /// <summary>
        /// Mask the source string with the default mask char.
        /// </summary>
        /// <param name="sourceValue">Original string to mask.</param>
        /// <returns>The masked account number.</returns>
        public static string Mask(this string sourceValue)
        {
            return Mask(sourceValue, DefaultMaskCharacter, 0, MaskStyle.All);
        }

        /// <summary>
        /// Mask the source string with the mask char.
        /// </summary>
        /// <param name="sourceValue">Original string to mask.</param>
        /// <param name="maskChar">The character to use to mask the source.</param>
        /// <param name="style">The masking style to use (all characters or just alpha-nums).</param>
        /// <returns>The masked account number.</returns>
        public static string Mask(this string sourceValue, char maskChar, MaskStyle style)
        {
            return Mask(sourceValue, maskChar, 0, style);
        }

        /// <summary>
        /// Mask the source string with the default mask char except for the last exposed digits.
        /// </summary>
        /// <param name="sourceValue">Original string to mask.</param>
        /// <param name="numExposed">Number of characters exposed in masked value.</param>
        /// <param name="style">The masking style to use (all characters or just alpha-nums).</param>
        /// <returns>The masked account number.</returns>
        public static string Mask(this string sourceValue, int numExposed, MaskStyle style)
        {
            return Mask(sourceValue, DefaultMaskCharacter, numExposed, style);
        }

        /// <summary>
        /// Mask the source string with the default mask char.
        /// </summary>
        /// <param name="sourceValue">Original string to mask.</param>
        /// <param name="style">The masking style to use (all characters or just alpha-nums).</param>
        /// <returns>The masked account number.</returns>
        public static string Mask(this string sourceValue, MaskStyle style)
        {
            return Mask(sourceValue, DefaultMaskCharacter, 0, style);
        }

        /// <summary>
        /// Masks all characters for the specified length.
        /// </summary>
        /// <param name="buffer">String builder to store result in.</param>
        /// <param name="source">The source string to pull non-alpha numeric characters.</param>
        /// <param name="mask">Masking character to use.</param>
        /// <param name="length">Length of the mask.</param>
        private static void CreateAlphaNumMask(StringBuilder buffer, string source, char mask, int length)
        {
            for (int i = 0; i < length; i++)
            {
                buffer.Append(char.IsLetterOrDigit(source[i])
                                ? mask
                                : source[i]);
            }
        }

        /// <summary>
        /// Default masking character used in a mask.
        /// </summary>
        public static readonly char DefaultMaskCharacter = '*';

        /// <summary>
        /// An enumeration of the types of masking styles for the Mask() extension method
        /// of the string class.
        /// </summary>
        public enum MaskStyle
        {
            /// <summary>
            /// Masks all characters within the masking region, regardless of type.
            /// </summary>
            All,

            /// <summary>
            /// Masks only alphabetic and numeric characters within the masking region.
            /// </summary>
            AlphaNumericOnly,
        }

        /// <summary>
        /// Returns true if the string is non-null and at least the specified number of characters.
        /// </summary>
        /// <param name="value">The string to check.</param>
        /// <param name="length">The minimum length.</param>
        /// <returns>True if string is non-null and at least the length specified.</returns>
        /// <exception>throws ArgumentOutOfRangeException if length is not a non-negative number.</exception>
        public static bool IsLengthAtLeast(this string value, int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), length,
                                                        "The length must be a non-negative number.");
            }

            return value != null
                        && value.Length >= length;
        }

        /// <summary>
        /// Count all words in a given string
        /// </summary>
        /// <param name="input">string to begin with</param>
        /// <returns>int</returns>
        public static int WordCount(this string input)
        {
            var count = 0;
            try
            {
                // Exclude whitespaces, Tabs and line breaks
                var re = new Regex(@"[^\s]+");
                var matches = re.Matches(input);
                count = matches.Count;
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
            }
            return count;
        }

        /// <summary>
        /// remove white space, not line end
        /// Useful when parsing user input such phone,
        /// price int.Parse("1 000 000".RemoveSpaces(),.....
        /// </summary>
        /// <param name="s"></param>
        /// <param name="value">string without spaces</param>
        public static string RemoveSpaces(this string s)
        {
            return s.Replace(" ", "");
        }

        /// <summary>
        /// Reduce string to shorter preview which is optionally ended by some string (...).
        /// </summary>
        /// <param name="s">string to reduce</param>
        /// <param name="count">Length of returned string including endings.</param>
        /// <param name="endings">optional edings of reduced text</param>
        /// <example>
        /// string description = "This is very long description of something";
        /// string preview = description.Reduce(20,"...");
        /// produce -> "This is very long..."
        /// </example>
        /// <returns></returns>
        public static string Reduce(this string s, int count, string endings)
        {
            if (count < endings.Length)
                throw new System.Exception("Failed to reduce to less then endings length.");
            int sLength = s.Length;
            int len = sLength;
            if (endings != null)
                len += endings.Length;
            if (count > sLength)
                return s; //it's too short to reduce
            s = s.Substring(0, sLength - len + count);
            if (endings != null)
                s += endings;
            return s;
        }

        /// <summary>
        /// Convert the string to Pascal case.
        /// </summary>
        /// <param name="theString">the string to turn into Pascal case</param>
        /// <returns>a string formatted as Pascal case</returns>
        public static string ToPascalCase(this string theString)
        {
            // If there are 0 or 1 characters, just return the string.
            if (theString == null) return theString;
            if (theString.Length < 2) return theString.ToUpper();

            // Split the string into words.
            string[] words = theString.Split(
                new char[] { },
                StringSplitOptions.RemoveEmptyEntries);

            // Combine the words.
            string result = "";
            foreach (string word in words)
            {
                result +=
                    word.Substring(0, 1).ToUpper() +
                    word.Substring(1);
            }

            return result;
        }

        /// <summary>
        /// Convert the string to camel case.
        /// </summary>
        /// <param name="theString">the string to turn into Camel case</param>
        /// <returns>a string formatted as Camel case</returns>
        public static string ToCamelCase(this string theString)
        {
            // If there are 0 or 1 characters, just return the string.
            if (theString == null || theString.Length < 2) return theString;

            // Split the string into words.
            string[] words = theString.Split(
                new char[] { },
                StringSplitOptions.RemoveEmptyEntries);

            // Combine the words.
            string result = words[0].ToLower();
            for (int i = 1; i < words.Length; i++)
            {
                result +=
                    words[i].Substring(0, 1).ToUpper() +
                    words[i].Substring(1);
            }

            return result;
        }

        /// <summary>
        /// Capitalize the first character and add a space before 
        /// each capitalized letter (except the first character). 
        /// </summary>
        /// <param name="theString">the string to turn into Proper case</param>
        /// <returns>a string formatted as Proper case</returns>
        public static string ToProperCase(this string theString)
        {
            // If there are 0 or 1 characters, just return the string.
            if (theString == null) return theString;
            if (theString.Length < 2) return theString.ToUpper();

            // Start with the first character.
            string result = theString.Substring(0, 1).ToUpper();

            // Add the remaining characters.
            for (int i = 1; i < theString.Length; i++)
            {
                if (Char.IsUpper(theString[i])) result += " ";
                result += theString[i];
            }

            return result;
        }

        private const string Newline = "\r\n";

        /// <summary>
        /// Word wraps the given text to fit within the specified width.
        /// </summary>
        /// <param name="text">Text to be word wrapped</param>
        /// <param name="theString"></param>
        /// <param name="width">Width, in characters, to which the text
        /// should be word wrapped</param>
        /// <returns>The modified text</returns>
        /// <see cref="http://www.softcircuits.com/Blog/post/2010/01/10/Implementing-Word-Wrap-in-C.aspx"/>
        public static string WordWrap(this string theString, int width)
        {
            int pos, next;
            StringBuilder sb = new StringBuilder();

            // Lucidity check
            if (width < 1)
                return theString;

            // Parse each line of text
            for (pos = 0; pos < theString.Length; pos = next)
            {
                // Find end of line
                int eol = theString.IndexOf(Newline, pos);

                if (eol == -1)
                    next = eol = theString.Length;
                else
                    next = eol + Newline.Length;

                // Copy this line of text, breaking into smaller lines as needed
                if (eol > pos)
                {
                    do
                    {
                        int len = eol - pos;

                        if (len > width)
                            len = BreakLine(theString, pos, width);

                        sb.Append(theString, pos, len);
                        sb.Append(Newline);

                        // Trim whitespace following break
                        pos += len;

                        while (pos < eol && Char.IsWhiteSpace(theString[pos]))
                            pos++;

                    } while (eol > pos);
                }
                else
                {
                    sb.Append(Newline); // Empty line
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Locates position to break the given line so as to avoid
        /// breaking words.
        /// </summary>
        /// <param name="text">String that contains line of text</param>
        /// <param name="pos">Index where line of text starts</param>
        /// <param name="max">Maximum line length</param>
        /// <returns>The modified line length</returns>
        public static int BreakLine(string text, int pos, int max)
        {
            // Find last whitespace in line
            int i = max - 1;

            while (i >= 0 && !Char.IsWhiteSpace(text[pos + i]))
                i--;

            if (i < 0)
                return max; // No whitespace found; break at maximum length

            // Find start of whitespace
            while (i >= 0 && Char.IsWhiteSpace(text[pos + i]))
                i--;

            // Return length of text before whitespace
            return i + 1;
        }

        /// <summary>
        /// Returns part of a string up to the specified number of characters, while maintaining full words
        /// </summary>
        /// <param name="s"></param>
        /// <param name="length">Maximum characters to be returned</param>
        /// <returns>String</returns>
        public static string Chop(this string s, int length)
        {
            if (String.IsNullOrEmpty(s))
                throw new ArgumentNullException(s);
            var words = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder();

            foreach (var word in words.Where(word => (sb.ToString().Length + word.Length) <= length))
            {
                sb.Append(word).Append(" ");
            }
            return sb.ToString().TrimEnd(' ') + "...";
        }

        public static string RightCharacter(this string str, int length)
        {
            try
            {
                return str.Substring(str.Length - length, length);
            }
            catch
            {
                return str.Substring(0);
            }
        }

        public static string LeftCharacter(this string param, int length)
        {
            int length1 = param.Length;
            return length1 >= length ? param.Substring(0, length) : param.Substring(0, length1);
        }

        public static string CapitalizeFirstLetter(this string s)
        {
            string asd = "ahmets";
            asd.RightCharacter(1);

            s = s.ToLower();
            if (string.IsNullOrEmpty(s))
                return s;
            if (s.Length == 1)
                return s.ToUpper();
            return s.Remove(1).ToUpper() + s.Substring(1);
        }

        public static string GetRandomString(int size)
        {
            StringBuilder stringBuilder = new StringBuilder();
            Random random = new Random();
            for (int index = 0; index < size; ++index)
            {
                char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26.0 * random.NextDouble() + 65.0)));
                stringBuilder.Append(ch);
            }
            return stringBuilder.ToString();
        }

        public static object ToPercentage(this object str)
        {
            object val = null;
            var percentage = Convert.ToDecimal(str);
            val = percentage.ToString("P", CultureInfo.InvariantCulture);
            return val;
        }

        public static object ToPercentage(this string str)
        {
            object val = null;
            var percentage = Convert.ToDecimal(str);
            val = percentage.ToString("P", CultureInfo.InvariantCulture);
            return val;
        }

        public static string FormatTextToHtml(string InputText)
        {
            ///TODO: HTML olarak düzenlenebilir
            InputText.Replace(Environment.NewLine, "<br />");
            InputText.Replace("\r", "<br />");
            return InputText.Replace("\n", "<br />");
        }

        #region PhoneReplace
        public static string ClearPhoneMask(this string phone)
        {
            try
            {
                if (phone.IsNotEmptyOrWhiteSpace())
                {
                    phone = phone.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");
                    return phone;
                }
                return "";
            }
            catch (Exception)
            {
                return "";
            }

        }
        #endregion
    }
}
