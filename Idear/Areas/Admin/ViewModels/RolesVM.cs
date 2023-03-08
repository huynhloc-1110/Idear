using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Idear.Areas.Admin.ViewModels
{
    public class RolesVM
    {
        public string? Id { get; set; }

        [Required]
        public string? RoleName { get; set; }
        public List<IdentityRole>? IdentityRoles { get; set; }
    }

}
