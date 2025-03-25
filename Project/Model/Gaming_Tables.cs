using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Project.Model
{
    public class Gaming_Tables
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }
        [DisplayName("Pit ID")]
        [BsonElement("pit_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Pit_id { get; set; }
        [DisplayName("Sub company ID")]
        [BsonElement("sub_company_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? sub_company_id { get; set; }
        [DisplayName("Table Number")]
        [Required]
        public string? table_number { get; set; }
        [DisplayName("Game Type")]
        [Required]
        public string? game_type { get; set; }
        [DisplayName("Minimum Bet")]
        [Required]
        public int? min_bet { get; set; }
        [DisplayName("Maximum Bet")]
        [Required]
        public int? max_bet { get; set; }
        [DisplayName("Chipset Configuration")]
        [Required]
        public Chips? chipset_configuration { get; set; }
        [DisplayName("Status")]
        [Required]
        public string? status { get; set; }
        [DisplayName("Create Time")]
        [Required]
        public DateTime? created_at { get; set; }
        [DisplayName("Update Time")]
        public DateTime? updated_at { get; set; }
    }
}
