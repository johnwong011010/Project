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
        [BsonElement("sub_company_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? sub_company_id { get; set; }
        [DisplayName("Name")]
        [Required]
        public string? name { get; set; }
        [DisplayName("Description")]
        public string? description { get; set; }
        [DisplayName("Create Time")]
        [Required]
        public DateTime? created_at { get; set; }
        [DisplayName("Update Time")]
        public DateTime? updated_at { get; set; }
    }
}
