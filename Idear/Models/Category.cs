using Microsoft.Build.Framework;

namespace Idear.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
