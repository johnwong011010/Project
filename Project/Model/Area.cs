using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Project.Model
{
    public class Area
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }
        [DisplayName("Sub-Company ID")]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Sub_company_id { get; set; }
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
