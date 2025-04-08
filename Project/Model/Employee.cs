using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Project.Model
{
    public class Employee
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; } //no change
        [Required]
        public string? Employee_Id { get; set; } //no change
        [Required]
        public string? Account { get; set; } //no change
        [Required]
        public string? Password { get; set; } //maybe
        [Required]
        public string? Name { get; set; } //no change
        [Required]
        public string? Role { get; set; } //always
        [Required]
        public DateTime? Join_in { get; set; } //few change
        [Required]
        public DateTime? Quit_in { get; set; } //few change
        [Required]
        public string? Permission { get; set; } //not always
        [Required]
        public string? Status { get; set; } //few change
        public RefreshToken? refreshToken { get; set; } //not allow to change
    }
}
