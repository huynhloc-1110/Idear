using System.ComponentModel.DataAnnotations;

namespace Idear.Models
{
    public class Department
    {
        public string? Id { get; set; }

        [StringLength(100)]
        [Required]
        public string? Name { get; set; }
        public List<ApplicationUser>? Users { get; set; }
    }
}
