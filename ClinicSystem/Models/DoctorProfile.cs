namespace ClinicSystem.Models
{
	public class DoctorProfile
	{
		public int Id { get; set; }
		public string AppUserId { get; set; } = null!;
		public AppUser AppUser { get; set; } = null!;
	}
}
