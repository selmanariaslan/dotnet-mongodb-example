using Arch.CoreLibrary.Entities;
using Arch.CoreLibrary.Utils.Security;
using Arch.Mongo.Managers;
using Arch.Mongo.Models.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Arch.Services.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class SecurityController : ControllerBase
    {
        [HttpGet(Name = "Encrypt")]
        public string Encrypt(string metin)
        {
            var crypto = new CryptoUtils();
            
            var result = crypto.EncryptToString(metin);
            return result;
        }

        [HttpGet(Name = "Decrypt")]
        public string Decrypt(string metin)
        {
            var crypto = new CryptoUtils();
            var result = crypto.DecryptString(metin);
            
            return result;
        }
    }
}
