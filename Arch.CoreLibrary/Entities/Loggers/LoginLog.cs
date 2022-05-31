using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arch.CoreLibrary.Entities.Log
{
    public class LoginLog : EntityBase
    {
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Ip { get; set; }
        public string Token { get; set; }
    }
}
