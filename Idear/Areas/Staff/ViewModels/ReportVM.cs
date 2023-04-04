using Idear.Models;
using System.ComponentModel.DataAnnotations;

namespace Idear.Areas.Staff.ViewModels
{
	public class ReportVM
	{
		[Required]
		[StringLength(500)]
		public string? Reason { get; set; }
		public string? ReportedCommentId { get; set; }
		public Comment? ReportedComment { get; set; }
		public string? ReportedIdeaId { get; set; }
		public Idea? ReportedIdea { get; set; }
	}
}
