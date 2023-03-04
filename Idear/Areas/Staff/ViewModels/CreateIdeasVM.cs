using Idear.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Idear.Areas.Staff.ViewModels
{
    public class CreateIdeasVM
    {
		public string? Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Text { get; set; }

        [Display(Name = "Image File")]
        public string? FilePath { get; set; }

        [Required]
        public string TopicId { get; set; } = default!;
        public List<SelectListItem>? Topics { get; set; } 

		[Required]
		public string CategoryId { get; set; } = default!;
		public List<SelectListItem>? Categories { get; set; }

        [Required]
        public bool IsAnonymous { get; set; }



		[Required]
		[IsTrue(ErrorMessage ="Please agree to Terms and Conditions")]        
		public bool AgreeTerms { get; set; }
	}

	public class IsTrueAttribute : ValidationAttribute
	{
		/// <summary>
		/// Determines whether the specified value of the object is valid. 
		/// </summary>
		/// <returns>
		/// true if the specified value is valid; otherwise, false. 
		/// </returns>
		/// <param name="value">The value of the specified validation object on which the <see cref="T:System.ComponentModel.DataAnnotations.ValidationAttribute"/> is declared.
		///	</param>
		public override bool IsValid(object? value)
		{
			if (value == null) return false;
			if (value.GetType() != typeof(bool)) throw new InvalidOperationException("can only be used on boolean properties.");

			return (bool)value;
		}
	}
}
