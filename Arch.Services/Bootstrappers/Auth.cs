using Arch.CoreLibrary.Utils.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Arch.Services.Bootstrappers
{
    public static class Auth
    {
        public static void AddJwtConfig(IServiceCollection services, IConfiguration configuration)
        {
            var secretKey = new CryptoUtils().DecryptString(configuration["JWT:Secret"]);
            var key = Encoding.ASCII.GetBytes(secretKey);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidAudience = configuration["JWT:Audience"],
                    ValidIssuer = configuration["JWT:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });
        }
    }
}
