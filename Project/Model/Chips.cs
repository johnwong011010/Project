using System.ComponentModel.DataAnnotations;

namespace Project.Model
{
    public class Chips
    {
        [Required]
        public List<string> chip_types { get; set; }
        [Required]
        public List<int> denominations { get; set; }
    }
}
