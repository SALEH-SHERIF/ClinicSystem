using System.ComponentModel.DataAnnotations;
using ClinicSystem.DTOs;

namespace ClinicSystem.Validations
{
	public class NationalityIdValidationAttribute : ValidationAttribute
	{
		protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
		{
			var model = validationContext.ObjectInstance as RegisterPatientDto;
			if (model == null)
				return new ValidationResult("Invalid object.");

			if (string.IsNullOrWhiteSpace(model.Nationality))
				return new ValidationResult("Nationality is required");

			if (model.Nationality == "Egyptian")
			{
				if (string.IsNullOrWhiteSpace(model.NationalId))
					return new ValidationResult("National ID is required for Egyptian patients.");

				if (!string.IsNullOrWhiteSpace(model.PassportNumber))
					return new ValidationResult("Passport number should not be provided for Egyptian patients.");
			}
			else if (model.Nationality == "Foreigner")
			{
				if (string.IsNullOrWhiteSpace(model.PassportNumber))
					return new ValidationResult("Passport number is required for foreign patients.");

				if (!string.IsNullOrWhiteSpace(model.NationalId))
					return new ValidationResult("National ID should not be provided for foreign patients.");
			}
			else
			{
				return new ValidationResult("Invalid nationality value. Must be 'Egyptian' or 'Foreigner'.");
			}

			return ValidationResult.Success;
		}
	}
}
