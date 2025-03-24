using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Project.Model
{
    public class operation_logs
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Gaming_Table_Id { get; set; }
        [DisplayName("Operation Type")]
        [Required]
        public string? Operation_Type { get; set; }
        [DisplayName("Performed User")]
        [Required]
        public string? Performed_user { get; set; }
        [DisplayName("Operation Timestamp")]
        [Required]
        public DateTime? op_timestamp { get; set; }
        [DisplayName("Details")]
        [Required]
        public string? details { get; set; }
    }
}
