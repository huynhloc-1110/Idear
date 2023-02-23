using System.ComponentModel.DataAnnotations;

namespace Idear.Models
{
    public class Category
    {
        public string? Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public List<Idea>? Ideas { get; set;}
    }
}
