using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arch.Mongo.Models.CommonModels
{
    public class UpsertModel
    {
        public int Id { get; set; }
        public bool Status { get; set; }
        public string Description { get; set; }
    }
}
