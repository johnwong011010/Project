using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Project.Model
{
    public class EmployeeDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }
        [Required]
        public string? Employee_Id { get; set; }
        [Required]
        public string? Account { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Role { get; set; }
        /*[Required]
        public DateTime? Join_in { get; set; }
        [Required]
        public DateTime? Quit_in { get; set; }*/
        [Required]
        public string? Permission { get; set; }
        [Required]
        public string? Status { get; set; }
    }
}
