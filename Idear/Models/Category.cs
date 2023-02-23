using System.ComponentModel.DataAnnotations;

namespace Idear.Models
{
    public class Category
    {
        public string? Id { get; set; }
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }
        public List<Idea>? Ideas { get; set;}
    }
}
