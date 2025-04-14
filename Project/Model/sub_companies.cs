using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Project.Model
{
    public class sub_companies
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }
        [DisplayName("Name")]
        [Required]
        public string? name { get; set; }
        [DisplayName("Description")]
        public string? description { get; set; }
        [DisplayName("Creat Time")]
        public DateTime? created_at { get; set; }
        [DisplayName("Update Time")]
        public DateTime? updated_at { get; set; }
    }
}
