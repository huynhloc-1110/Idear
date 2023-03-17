using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Idear.Models
{
    public class Topic
    {
        public string? Id { get; set; }
        [Required]
        [StringLength(500)]
        public string? Name { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Closure Date")]
        public DateTime ClosureDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Final Closure Date")]
        [NotBefore(nameof(ClosureDate))]
        public DateTime FinalClosureDate { get; set; }

        public List<Idea>? Ideas { get; set; }
    }

	public class NotBeforeAttribute : ValidationAttribute, IClientModelValidator
	{
		private readonly string _closureDateFieldName;

		public NotBeforeAttribute(string closureDateFieldName)
		{
			_closureDateFieldName = closureDateFieldName;
		}

		protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
		{
			var closureDateProperty = validationContext.ObjectType.GetProperty(_closureDateFieldName);
			if (closureDateProperty == null)
			{
				throw new ArgumentException("Invalid property name");
			}

			var closureDate = (DateTime)closureDateProperty.GetValue(validationContext.ObjectInstance)!;
			var finalClosureDate = (DateTime)value!;
			if (finalClosureDate < closureDate)
			{
				return new ValidationResult(GetErrorMessage());
			}

			return ValidationResult.Success;
		}

		public void AddValidation(ClientModelValidationContext context)
		{
			MergeAttribute(context.Attributes, "data-val", "true");
			MergeAttribute(context.Attributes, "data-val-notbefore", GetErrorMessage());
			MergeAttribute(context.Attributes, "data-val-notbefore-closuredate", $"#Topic_{_closureDateFieldName}");
		}

		private static string GetErrorMessage()
		   => "Final Closure Date must not before Closure Date";

		private static bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
		{
			if (attributes.ContainsKey(key))
			{
				return false;
			}

			attributes.Add(key, value);
			return true;
		}
	}
}
