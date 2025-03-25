using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Project.Model
{
    public class Zone
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }
        [DisplayName("Area ID")]
        [BsonElement("Area_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? area_id { get; set; }
        [DisplayName("Name")]
        [Required]
        public string? name { get; set; }
        [DisplayName("Description")]
        public string? description { get; set; }
        [DisplayName("Create Time")]
        [Required]
        public DateTime? created_at { get; set; }
        [DisplayName("Update Time")]
        public DateTime? update_at { get; set; }
    }
}
