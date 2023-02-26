using Microsoft.Build.Framework;

namespace Idear.ViewModels
{
	public class RolesVM
	{
		public string? Id { get; set; }
		[Required]
		public string? RoleName { get; set; }
	}
}
