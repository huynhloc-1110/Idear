using System.ComponentModel.DataAnnotations;

namespace Idear.Models
{
    public class Comment
    {
        public string? Id { get; set; }
        [Required]
        [StringLength(500)]
        public string? Text { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Date Time")]
        public DateTime Datetime { get; set; }

        [Required]
        public ApplicationUser? User { get; set; }

        [Required]
        public Idea? Idea { get; set; }
    }
}
