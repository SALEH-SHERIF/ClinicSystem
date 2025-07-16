using System.ComponentModel.DataAnnotations;

public class BirthDateValidationAttribute : ValidationAttribute
{
	public int MinAge { get; set; } = 0;
	public int MaxAge { get; set; } = 120;

	protected override ValidationResult IsValid(object value, ValidationContext validationContext)
	{
		if (value is not DateTime birthDate)
			return new ValidationResult("Invalid date format");

		var today = DateTime.Today;

		if (birthDate > today)
			return new ValidationResult("Birthdate cannot be in the future");

		var age = today.Year - birthDate.Year;
		if (birthDate > today.AddYears(-age)) age--;

		if (age < MinAge || age > MaxAge)
			return new ValidationResult($"Age must be between {MinAge} and {MaxAge} years");

		return ValidationResult.Success;
	}
}
