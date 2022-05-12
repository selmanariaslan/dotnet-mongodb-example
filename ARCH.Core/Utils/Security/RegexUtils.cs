using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ARCH.CoreLibrary.Utils.Security
{

    public static class RegexUtils
    {
        public static bool IsValidEmailAddress(this string s)
        {
            Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            return regex.IsMatch(s);
        }

        /*
         var htmlText = "<p>Here is some text. <span class="bold">This is bold.</span> Talk to you later.</p>;
         var cleanString = htmlText.StripHtml();
         */
        public static string StripHtml(this string input)
        {
            // Will this simple expression replace all tags???
            var tagsExpression = new Regex("</?.+?>");
            return tagsExpression.Replace(input, " ");
        }

        public static bool IsGuid(this string s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            Regex format = new Regex(
                "^[A-Fa-f0-9]{32}$|" +
                "^({|\\()?[A-Fa-f0-9]{8}-([A-Fa-f0-9]{4}-){3}[A-Fa-f0-9]{12}(}|\\))?$|" +
                "^({)?[0xA-Fa-f0-9]{3,10}(, {0,1}[0xA-Fa-f0-9]{3,6}){2}, {0,1}({)([0xA-Fa-f0-9]{3,4}, {0,1}){7}[0xA-Fa-f0-9]{3,4}(}})$");
            Match match = format.Match(s);

            return match.Success;
        }

        public static bool IsValidUrl(this string text)
        {
            Regex rx = new Regex(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
            return rx.IsMatch(text);
        }

        public static bool UrlAvailable(this string httpUrl)
        {
            if (!httpUrl.StartsWith("http://") || !httpUrl.StartsWith("https://"))
                httpUrl = "http://" + httpUrl;
            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(httpUrl);
                myRequest.Method = "GET";
                myRequest.ContentType = "application/x-www-form-urlencoded";
                HttpWebResponse myHttpWebResponse =
                   (HttpWebResponse)myRequest.GetResponse();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string ToUrlSlug(this string text)
        {
            return Regex.Replace(
                        Regex.Replace(
                            Regex.Replace(
                                text.Trim().ToLower()
                                        .Replace("ö", "o")
                                        .Replace("ç", "c")
                                        .Replace("ş", "s")
                                        .Replace("ı", "i")
                                        .Replace("ğ", "g")
                                        .Replace("ü", "u"),
                            @"\s+", " "), // multiple spaces to one space
                            @"\s", "-"), // spaces to hypens
                            @"[^a-z0-9\s-]", ""); // removing invalid chars
        }

        public static bool IsValidIpAddress(this string s)
        {
            return Regex.IsMatch(s,
                    @"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b");
        }

        public static bool IsUnicode(this string value)
        {
            int asciiBytesCount = Encoding.ASCII.GetByteCount(value);
            int unicodBytesCount = Encoding.UTF8.GetByteCount(value);

            return asciiBytesCount != unicodBytesCount;
        }

        public static bool IsStrongPassword(this string s)
        {
            bool isStrong = Regex.IsMatch(s, @"[\d]");
            if (isStrong) isStrong = Regex.IsMatch(s, "[a-z]");
            if (isStrong) isStrong = Regex.IsMatch(s, "[A-Z]");
            if (isStrong) isStrong = Regex.IsMatch(s, @"[\s~!@#\$%\^&\*\(\)\{\}\|\[\]\\:;'?,.`+=<>\/]");
            if (isStrong) isStrong = s.Length > 7;
            return isStrong;
        }

        public static string CleanHtmlTags(this object o)
        {
            return Regex.Replace(o.ToString(), @"<(.|\n)*?>", string.Empty);
        }
    }
}
