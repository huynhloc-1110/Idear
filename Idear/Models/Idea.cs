using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Idear.Models
{
	public class Idea
	{
		public ApplicationUser? User { get; set; }
		public Topic? Topic { get; set; }

		public Category? Category { get; set; }


		public string? Id { get; set; }
		[Required]
		[StringLength(500)]
		public string? Text { get; set; }
		public string? FilePath { get; set; }

		[DataType(DataType.Date)]
		public DateTime DateTime { get; set; }

	}
}
