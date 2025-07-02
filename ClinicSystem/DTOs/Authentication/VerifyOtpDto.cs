using System.ComponentModel.DataAnnotations;

namespace ClinicSystem.DTOs
{
	public class VerifyOtpDto
	{
		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid email format")]
		public string Email { get; set; }

		[Required(ErrorMessage ="Please Enter Code")]
		public string Code { get; set; }
	}
}
