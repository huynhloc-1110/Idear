using System.ComponentModel.DataAnnotations;

namespace Idear.Models
{
    public class Comment
    {
        public string? Id { get; set; }

        private string? text;

        [Required]
        [StringLength(500)]
        public string? Text
        {
            get
            {
                if (IsHidden)
                {
                    return "*This comment is reported and hidden by a QA Manager.";
                }
                return text;
            }
            set { text = value; }
        }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Date Time")]
        public DateTime Datetime { get; set; }

        public bool IsAnonymous { get; set; }
        public bool IsHidden { get; set; } = false;
        public ApplicationUser? User { get; set; }
        public Idea? Idea { get; set; }
    }
}
