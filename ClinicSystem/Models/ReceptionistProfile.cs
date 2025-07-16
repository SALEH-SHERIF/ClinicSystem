namespace ClinicSystem.Models
{
	public class ReceptionistProfile
	{
		public int Id { get; set; }
		public string AppUserId { get; set; } = null!;
		public AppUser AppUser { get; set; } = null!;
	}
}
