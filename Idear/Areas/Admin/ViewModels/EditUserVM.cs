using Idear.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Idear.Areas.Admin.ViewModels
{
    public class EditUserVM
    {
        [Required]
        public string? Id { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [StringLength(100)]
        public string? FullName { get; set; }

        [Required]
        public string? DepartmentId { get; set; }

        [ValidateNever]
        public List<SelectListItem>? Departments { get; set; }
    }
}
