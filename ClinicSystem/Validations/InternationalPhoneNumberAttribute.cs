using PhoneNumbers;
using System.ComponentModel.DataAnnotations;

public class InternationalPhoneNumberAttribute : ValidationAttribute
{
	protected override ValidationResult IsValid(object value, ValidationContext validationContext)
	{
		var phoneNumber = value as string;
		if (string.IsNullOrEmpty(phoneNumber))
			return new ValidationResult("Phone number is required");

		try
		{
			var phoneUtil = PhoneNumberUtil.GetInstance();
			var parsed = phoneUtil.Parse(phoneNumber, null); 

			if (phoneUtil.IsValidNumber(parsed))
				return ValidationResult.Success;

			return new ValidationResult("Invalid phone number format");
		}
		catch (NumberParseException)
		{
			return new ValidationResult("Invalid phone number format");
		}
	}
}
