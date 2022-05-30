using Arch.CoreLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arch.CoreLibrary.Utils.Collections
{
    public class EntityComparer : EqualityComparer<EntityBase>
    {
        public override bool Equals(EntityBase x, EntityBase y)
        {
            return x.Id == y.Id;
        }

        public override int GetHashCode(EntityBase obj)
        {
            return obj?.Id ?? 0;
        }
    }
}
