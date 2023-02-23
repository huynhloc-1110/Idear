using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;

namespace Idear.Models
{
    public class View
    {
        public string? Id { get; set; }
        public int VisitTime { get; set; }
        public ApplicationUser? User { get; set; }
        public Idea? idea { get; set; }
    }
}
