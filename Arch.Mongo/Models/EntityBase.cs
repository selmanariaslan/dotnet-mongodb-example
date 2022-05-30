using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arch.Mongo.Models
{
    public interface IEntityBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        string Id { get; set; }

        DateTime CreatedAt { get; }
        string CreatedBy { get; set; }

        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        DateTime? ModifiedDate { get; set; }

        string ModifiedBy { get; set; }
    }
    public abstract class EntityBase : IEntityBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public DateTime CreatedAt => DateTime.UtcNow;
        public string CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }
    }
}
