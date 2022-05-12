using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARCH.CoreLibrary.Entities.CommonModels
{
    public class CommonInsertModel : BaseRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
