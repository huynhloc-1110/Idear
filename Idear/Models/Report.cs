using System.ComponentModel.DataAnnotations;

namespace Idear.Models
{
	public class Report
	{
		public string? Id { get; set; }
		[Required]
		[StringLength(500)]
		public string? Reason { get; set; }
		[Required]
		public DateTime DateTime { get; set; }
		public Comment? ReportedComment { get; set; }
		public Idea? ReportedIdea { get; set; }
		public ApplicationUser? Reporter { get; set; }
	}
}
