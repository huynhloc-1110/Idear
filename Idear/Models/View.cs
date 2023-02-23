using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;

namespace Idear.Models
{
    public class View
    {
        public string? Id { get; set; }
        [Required]
        public int VisitTime { get; set; }
        [Required]
        public ApplicationUser? User { get; set; }
        [Required]
        public Idea? Idea { get; set; }
    }
}
