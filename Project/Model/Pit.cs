using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Project.Model
{
    public class Pit
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }
        [DisplayName("Zone ID")]
        [BsonElement("zone_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? zone_id { get; set; }
        [DisplayName("Name")]
        public string? name { get; set; }
        [DisplayName("Description")]
        public string? description { get; set; }
        [DisplayName("Create Time")]
        [Required]
        public DateTime? created_at { get; set; }
        [DisplayName("Update Time")]
        public DateTime? updated_at { get; set; }
        [DisplayName("isDeleted")]
        public string? isDeleted { get; set; }
    }
}
