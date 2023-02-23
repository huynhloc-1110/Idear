using System.ComponentModel.DataAnnotations;

namespace Idear.Models
{
    public class Comment
    {
        public string? Id { get; set; }
        [Required]
        [StringLength(500)]
        public string? Text { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Date Time")]
        public DateTime Datetime { get; set; }
        public ApplicationUser? User { get; set; }
        public Idea? idea { get; set; }
    }
}
