using ClinicSystem.Validations;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;
namespace ClinicSystem.DTOs
{
	[NationalityIdValidation]
	public class RegisterPatientDto
	{
		[Required(ErrorMessage = "Arabic full name is required")]
		[StringLength(30, MinimumLength = 5, ErrorMessage = "Name must be between 5 and 30 characters")]
		[RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = "Arabic name must contain only Arabic letters and spaces")]
		public string ArabicFullName { get; set; } = string.Empty;

		[Required(ErrorMessage = "English full name is required")]
		[StringLength(30, MinimumLength = 5, ErrorMessage = "Name must be between 5 and 30 characters")]
		[RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "English name must contain only letters and spaces")]
		public string EnglishFullName { get; set; } = string.Empty;

		[Required(ErrorMessage = "Birthdate is required")]
		[DataType(DataType.Date)]
		[BirthDateValidation(MinAge = 0, MaxAge = 120)]
		public DateTime BirthDate { get; set; }

		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid email format")]
		public string Email { get; set; } = string.Empty;

		[Required(ErrorMessage = "Phone number is required")]
		[InternationalPhoneNumber]
		public string PhoneNumber { get; set; } = string.Empty;

		[Required(ErrorMessage = "Nationality is required")]
		[RegularExpression("^(Egyptian|Foreigner)$", ErrorMessage = "Nationality must be either 'Egyptian' or 'Foreigner'")]
		public string Nationality { get; set; } = string.Empty;

		[StringLength(14, MinimumLength = 14, ErrorMessage = "National ID must be exactly 14 characters.")]
		[RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must contain only digits.")]
		public string? NationalId { get; set; }

		[StringLength(9, MinimumLength = 6, ErrorMessage = "Passport number must be between 6 and 9 characters.")]
		[RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "Passport number can only contain letters and digits.")]
		public string? PassportNumber { get; set; }

		[Required(ErrorMessage = "Password is required")]
		[StringLength(15, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 15 characters")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
			ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Please confirm your password")]
		[Compare("Password", ErrorMessage = "Passwords do not match")]
		public string ConfirmPassword { get; set; }
	}
}
