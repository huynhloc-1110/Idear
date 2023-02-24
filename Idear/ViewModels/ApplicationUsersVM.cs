using Idear.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;

namespace Idear.ViewModels
{
	public class ApplicationUsersVM
	{
		
		public ApplicationUser? AppUser { get; set; }

		[Required]
		public List<SelectListItem>? roles { get; set; }

		[Required]
		public string? department { get; set; }

	}
}
