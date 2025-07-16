using Microsoft.AspNetCore.Identity;

namespace ClinicSystem.Models
{
	public class AppUser : IdentityUser
	{
		public string FullName { get; set; } = string.Empty; // english 
		public string FullNameArabic {  get; set; } = string.Empty; // arabic 
		public string? Nationality { get; set; } // "Egyptian" or "Foreigner"
		public string? NationalId { get; set; }  // If Egyptian
		public string? PassportNumber { get; set; } // If Foreigner
		public DateTime BirthDate { get; set; }

		// Navigation
		public PatientProfile? PatientProfile { get; set; }
		public DoctorProfile? DoctorProfile { get; set; }
		public ReceptionistProfile? ReceptionistProfile { get; set; }
		public ICollection<UserOtp> Otps { get; set; } = new List<UserOtp>();
	}
}

