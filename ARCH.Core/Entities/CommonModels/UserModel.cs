using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARCH.CoreLibrary.Entities.CommonModels
{
    public class UserModel
    {
        public UserModel()
        {
            Roles = new List<string>();
            CompanyIds = new List<int>();
            Modules = new List<string>();
        }
        public int Id { get; set; }
        public int CurrentCompanyId { get; set; }
        public int ParentCompanyId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public bool IsExternal { get; set; }
        public bool? IsChangePassword { get; set; }
        public bool? IsCrm { get; set; }

        public List<string> Roles { get; set; }
        public List<int> CompanyIds { get; set; }
        public List<string> Modules { get; set; }

        public string FullName { get { return $"{FirstName} {LastName}"; } }
    }
}
