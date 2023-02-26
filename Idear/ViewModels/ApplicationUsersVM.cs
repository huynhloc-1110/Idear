using Idear.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;

namespace Idear.ViewModels
{
	public class ApplicationUsersVM
	{
		public ApplicationUser AppUser { get; set; } = default!;

		[Required]
		public List<SelectListItem> Roles { get; set; } = default!;

	}
}
