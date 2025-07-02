using System.ComponentModel.DataAnnotations;

namespace ClinicSystem.DTOs.Authentication
{
    public class ForgetPasswordDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
    }
}
