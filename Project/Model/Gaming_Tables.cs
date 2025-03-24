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
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Pit_id { get; set; }
        [DisplayName("Sub company ID")]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? sub_company_id { get; set; }
        [DisplayName("Table Number")]
        [Required]
        public string? Table_number { get; set; }
        [DisplayName("Game Type")]
        [Required]
        public string? Game_type { get; set; }
        [DisplayName("Minimum Bet")]
        [Required]
        public int? Min_bet { get; set; }
        [DisplayName("Maximum Bet")]
        [Required]
        public int? Max_bet { get; set; }
        [DisplayName("Chipset Configuration")]
        [Required]
        public Chips chipset { get; set; }
        [DisplayName("Status")]
        [Required]
        public string? Status { get; set; }
        [DisplayName("Create Time")]
        [Required]
        public DateTime? Create_time { get; set; }
        [DisplayName("Update Time")]
        public DateTime? Update_time { get; set; }
    }
}
