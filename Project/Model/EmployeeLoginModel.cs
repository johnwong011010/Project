using System.ComponentModel.DataAnnotations;

namespace Project.Model
{
    public class EmployeeLoginModel
    {
        [Required]
        public string? Account { get; set; }
        [Required]
        public string? Password { get; set; }
        public string? Detail { get; set; }
        public string? Token { get; set; }
    }
}
