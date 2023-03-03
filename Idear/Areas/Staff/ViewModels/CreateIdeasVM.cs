using Idear.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Idear.Areas.Staff.ViewModels
{
    public class CreateIdeasVM
    {
        public string Id { get; set; }
        [Required]
        [StringLength(500)]
        public string Text { get; set; }

        [Display(Name = "Image File")]
        public string FilePath { get; set; }

        public string TopicId { get; set; }
        public List<SelectListItem> Topics { get; set; } = default!;

        public string CategoryId { get; set; }
        public List<SelectListItem> Categories { get; set; } = default!;

        public bool IsAnonymous { get; set; }
    }
}
