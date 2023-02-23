using System.ComponentModel.DataAnnotations;

namespace Idear.Models
{
	public class React
	{
		public string Id { get; set; }
		public ApplicationUser? User { get; set; }
		public Idea? Idea { get; set; }


		//Specifies a numeric minimum and maximum range for a property
		[Range(-1, 1)]
		public int ReactFlag { get; set; }

	}
}
