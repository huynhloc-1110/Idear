using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Idear.Models
{
	public class Idea
	{
		public string? Id { get; set; }
		[Required]
		[StringLength(500)]
		public string? Text { get; set; }
		public string? FilePath { get; set; }

		[Required]
		[DataType(DataType.Date)]
		public DateTime DateTime { get; set; }

        public ApplicationUser? User { get; set; }

        public Topic? Topic { get; set; }

        public Category? Category { get; set; }
        public bool IsAnonymous { get; set; }
        public List<Comment>? Comments { get; set; }
        public List<View>? Views { get; set; }
		public List<React>? Reacts { get; set; }

    }
}
