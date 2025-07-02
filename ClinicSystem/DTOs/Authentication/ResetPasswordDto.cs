using System.ComponentModel.DataAnnotations;

namespace ClinicSystem.DTOs
{
	public class ResetPasswordDto
	{
		[Required(ErrorMessage ="Please Enter Email "), EmailAddress(ErrorMessage ="Please Enter Valid Email")]
		public string Email { get; set; }

		[Required(ErrorMessage ="Please Enter Code")]
		public string OtpCode { get; set; }

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
