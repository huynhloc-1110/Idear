using Idear.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Idear.Areas.Admin.ViewModels
{
    public class ApplicationUsersVM
    {
        public ApplicationUser AppUser { get; set; } = default!;

        [Required]
        public List<SelectListItem> Roles { get; set; } = default!;

    }
}
