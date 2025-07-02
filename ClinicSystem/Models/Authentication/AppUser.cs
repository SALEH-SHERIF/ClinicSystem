using Microsoft.AspNetCore.Identity;

namespace ClinicSystem.Models
{
	public class AppUser : IdentityUser
	{
		public string FullName { get; set; } = string.Empty;
		public string? Nationality { get; set; } // "Egyptian" or "Foreigner"
		public string? NationalId { get; set; }  // If Egyptian
		public string? PassportNumber { get; set; } // If Foreigner

		public ICollection<UserOtp> Otps { get; set; } = new List<UserOtp>();
	}
}

