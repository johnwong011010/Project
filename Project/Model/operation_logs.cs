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
        [BsonElement("Gaming_Table_Id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? gaming_table_id { get; set; }
        [DisplayName("Operation Type")]
        [Required]
        public string? operation_type { get; set; }
        [DisplayName("Performed User")]
        [Required]
        public string? performed_by { get; set; }
        [DisplayName("Operation Timestamp")]
        [Required]
        public DateTime? operation_timestamp { get; set; }
        [DisplayName("Details")]
        [Required]
        public string? details { get; set; }
    }
}
