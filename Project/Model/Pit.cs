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
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Zone_id { get; set; }
        [DisplayName("Name")]
        [Required]
        public string? Name { get; set; }
        [DisplayName("Description")]
        public string? Description { get; set; }
        [DisplayName("Create Time")]
        [Required]
        public DateTime? Create_Time { get; set; }
        [DisplayName("Update Time")]
        public DateTime? Update_Time { get; set; }
    }
}
