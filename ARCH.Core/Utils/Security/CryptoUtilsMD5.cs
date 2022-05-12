using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARCH.CoreLibrary.Utils.Security
{
    public static class CryptoUtilsMD5
    {
        public static string ConvertMD5(string text)
        {
            //byte[] hash = new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(text));
            StringBuilder stringBuilder = new StringBuilder();
            //foreach (byte num in hash)
            //    stringBuilder.Append(num.ToString("x2").ToLower());
            return stringBuilder.ToString();
        }
    }
}
