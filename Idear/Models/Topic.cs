using System.ComponentModel.DataAnnotations;

namespace Idear.Models
{
    public class Topic
    {
        public string? Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public DateTime ClosureDate { get; set; }
        [Required]
        public DateTime FinalClosureTime { get; set; }
    }
}
