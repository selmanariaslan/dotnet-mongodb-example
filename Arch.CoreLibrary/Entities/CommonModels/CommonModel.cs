using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arch.CoreLibrary.Entities.CommonModels
{
    public class CommonModel : BaseRequest
    {
        public int? Id { get; set; }
        public string Text { get; set; }
    }
}
