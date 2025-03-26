using System.ComponentModel.DataAnnotations;

namespace Project.Model
{
    public class TokenRefresh
    {
        [Required]
        public string? Token { get; set; }
        [Required]
        public string? RefreshToken { get; set; }
    }
}
