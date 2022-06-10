using Arch.CoreLibrary.Utils;
using Arch.CoreLibrary.Utils.Security;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arch.Data
{    public static class DatabaseConfig
    {
        public static string GetConnectionString(string database = "CurrentDatabase", bool isEncryptedString = true)
        {
            var root = ConfigUtils.GetConfigurationRoot();

            var sqlConnection = root.GetConnectionString(root.GetSection("Application").GetSection("Database")[database]);

            if (sqlConnection is not null)
            {
                if (isEncryptedString)
                {
                    var crypto = new CryptoUtils();
                    return crypto.DecryptString(sqlConnection);
                }
                return sqlConnection;
            }
            else
            {
                return null;
            }
        }
    }
}
