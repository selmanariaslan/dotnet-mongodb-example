using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARCH.CoreLibrary.Entities.CommonModels
{
    public class CommonUpdateModel : BaseRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        private int? _status;
        public int Status
        {
            get { return _status ?? 1; }
            set { _status = value; }
        }
    }
}
