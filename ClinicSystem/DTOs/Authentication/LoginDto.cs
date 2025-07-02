using System.ComponentModel.DataAnnotations;

namespace ClinicSystem.DTOs
{
	public class LoginDto
	{
		[Required(ErrorMessage ="Please Enter Email")]
		[EmailAddress(ErrorMessage ="Please Enter Valid Email")]
		public string Email { get; set; } = string.Empty;

		[Required(ErrorMessage ="Please Enter Password")]
		public string Password { get; set; } = string.Empty;
	}
}
