using Idear.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

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
		[IsTrue(ErrorMessage = "Please agree to Terms and Conditions")]        
		public bool AgreeTerms { get; set; }

		[Required]
		public bool DeleteCurrentFile { get; set; }
	}

	public class IsTrueAttribute : ValidationAttribute, IClientModelValidator
	{
        public void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-istrue", GetErrorMessage());
        }

        private static string GetErrorMessage()
            => "Please agree to Terms and Conditions";

        private static bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }

            attributes.Add(key, value);
            return true;
        }

        public override bool IsValid(object? value)
		{
			if (value == null) return false;
			if (value.GetType() != typeof(bool)) throw new InvalidOperationException("can only be used on boolean properties.");

			return (bool)value;
		}
	}
}
