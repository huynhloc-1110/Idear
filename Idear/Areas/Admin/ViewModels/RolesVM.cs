using Microsoft.Build.Framework;

namespace Idear.Areas.Admin.ViewModels
{
    public class RolesVM
    {
        public string? Id { get; set; }
        [Required]
        public string? RoleName { get; set; }
    }
}
