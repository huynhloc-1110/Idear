using System.ComponentModel.DataAnnotations;

namespace Idear.Models
{
	public class React
	{
		public string? Id { get; set; }
		[Required]
		public ApplicationUser? User { get; set; }
		[Required]
		public Idea? Idea { get; set; }


		//Specifies a numeric minimum and maximum range for a property
		[Required]
		[Range(-1, 1)]
		public int ReactFlag { get; set; }

	}
}
