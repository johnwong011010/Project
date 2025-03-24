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
        public string? Name { get; set; }
        [DisplayName("Description")]
        public string? Description { get; set; }
        [DisplayName("Creat Time")]
        [Required]
        public DateTime? CreateTime { get; set; }
        [DisplayName("Update Time")]
        public DateTime? UpdateTime { get; set; }
    }
}
