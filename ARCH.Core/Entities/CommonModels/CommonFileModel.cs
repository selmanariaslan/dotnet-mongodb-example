using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARCH.CoreLibrary.Entities.CommonModels
{
    public class CommonFileModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string FolderName { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string FileOriginalName { get; set; }
        public string FullPath { get; set; }
        public long FileSize { get; set; }
        public int Status { get; set; }
    }
}
