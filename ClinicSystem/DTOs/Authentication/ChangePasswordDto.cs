using System.ComponentModel.DataAnnotations;

namespace ClinicSystem.DTOs.Authentication
{
	public class ChangePasswordDto
	{

		[Required(ErrorMessage = "Password is required")]
		[StringLength(15, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 15 characters")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
			ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character")]
		public string OldPassword { get; set; }

		[Required(ErrorMessage = "Password is required")]
		[StringLength(15, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 15 characters")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
			ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character")]
		public string NewPassword { get; set; }
	}
}
