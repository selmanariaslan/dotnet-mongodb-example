using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Arch.CoreLibrary.Utils
{
    public static class ConfigUtils
    {
        private static IConfigurationRoot _configurationRoot;

        public static IConfigurationRoot GetConfigurationRoot()
        {
            if (_configurationRoot == null)
            {
                var configurationBuilder = new ConfigurationBuilder();
                var path = "appsettings.json".ToApplicationPath();
                configurationBuilder.AddJsonFile(path, false);

                _configurationRoot = configurationBuilder.Build();
            }

            return _configurationRoot;
        }

        public static string ToApplicationPath(this string fileName)
        {
            var exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathMatcher.Match(exePath).Value;
            return Path.Combine(appRoot, fileName);
        }

        public static string GetCurrentMethodName(int upperLevel)
        {
            string retval = null;
            StackTrace stackTrace = new StackTrace();
            var reflectedType = stackTrace.GetFrame(upperLevel).GetMethod().ReflectedType;
            if (reflectedType != null)
                retval = reflectedType.FullName + "." + stackTrace.GetFrame(upperLevel).GetMethod().Name;
            return retval;
        }
    }
}
