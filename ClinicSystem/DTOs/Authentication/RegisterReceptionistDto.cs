using System.ComponentModel.DataAnnotations;

namespace ClinicSystem.DTOs
{
	public class RegisterReceptionistDto
	{
		[Required(ErrorMessage = "Full name is required")]
		[StringLength(30, MinimumLength = 5, ErrorMessage = "Full name must be between 5 and 30 characters")]
		[RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Full name must contain only letters and spaces")]
		public string FullName { get; set; }

		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid email format")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Phone number is required")]
		[InternationalPhoneNumber]
		public string PhoneNumber { get; set; }

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
