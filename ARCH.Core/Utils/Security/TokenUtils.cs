using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ARCH.CoreLibrary.Utils.Security
{
    public static class TokenUtils
    {
        public static string GenerateJwtToken(string userId, string userRole, int expires = 7, string secretKey = "Secret", bool isEncryptedSecretKey = true)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var appConfigRoot = ConfigUtils.GetConfigurationRoot();

            string sKey = appConfigRoot.GetSection("Application")[secretKey];
            if (isEncryptedSecretKey)
            {
                sKey = new CryptoUtils().DecryptString(sKey);
            }

            var key = Encoding.ASCII.GetBytes(sKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userId),
                    new Claim(ClaimTypes.Role, userRole ?? "")
                }),
                Expires = DateTime.UtcNow.AddDays(expires),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
