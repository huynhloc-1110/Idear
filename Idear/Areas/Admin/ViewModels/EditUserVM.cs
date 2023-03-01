using Idear.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;

namespace Idear.Areas.Admin.ViewModels
{
    public class EditUserVM
    {
        [Required]
        public string? Id { get; set; }

        public string? Email { get; set; }
        public string? FullName { get; set; }

        [Required]
        public string? DepartmentId { get; set; }
        public List<SelectListItem> Departments { get; set; } = default!;
    }
}
