using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ClinicSystem.Models.Enums;

namespace ClinicSystem.Models
{
    public class UserOtp
	{
		[Key]
		public int Id { get; set; }

		public string Code { get; set; } = string.Empty;

		public DateTime ExpirationTime { get; set; }

		public bool IsUsed { get; set; } = false;

		// Foreign Key
		public string UserId { get; set; }

		//[ForeignKey("UserId")]
		public AppUser User { get; set; }
		public OtpPurpose Purpose { get; set; }
	}
}
