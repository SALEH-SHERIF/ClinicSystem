using ClinicSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ClinicSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{

		}
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<PatientProfile>()
				.HasOne(p => p.AppUser)
				.WithOne(u => u.PatientProfile)
				.HasForeignKey<PatientProfile>(p => p.AppUserId);

			builder.Entity<DoctorProfile>()
				.HasOne(p => p.AppUser)
				.WithOne(u => u.DoctorProfile)
				.HasForeignKey<DoctorProfile>(p => p.AppUserId);

			builder.Entity<ReceptionistProfile>()
				.HasOne(p => p.AppUser)
				.WithOne(u => u.ReceptionistProfile)
				.HasForeignKey<ReceptionistProfile>(p => p.AppUserId);
		}

		public DbSet<UserOtp> UserOtps { get; set; }
		public DbSet<PatientProfile> PatientProfiles { get; set; }
		public DbSet<DoctorProfile> DoctorProfiles { get; set; }
		public DbSet<ReceptionistProfile> ReceptionistProfiles { get; set; }

	}
}
