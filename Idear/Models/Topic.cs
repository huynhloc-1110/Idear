using System.ComponentModel.DataAnnotations;

namespace Idear.Models
{
    public class Topic
    {
        public string? Id { get; set; }
        [Required]
        [StringLength(500)]
        public string? Name { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Closure Date")]
        public DateTime ClosureDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Final Closure Date")]
        public DateTime FinalClosureDate { get; set; }

        public List<Idea>? Ideas { get; set; }
    }
}
