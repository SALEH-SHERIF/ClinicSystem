using ClinicSystem.Data;
using ClinicSystem.Interfaces;
using ClinicSystem.Models;

namespace ClinicSystem.Repositories
{
	public class ReceptionistRepository : IReceptionistRepository
	{
		private readonly ApplicationDbContext _context;

		public ReceptionistRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task CreateAsync(ReceptionistProfile receptionist)
		{
			_context.ReceptionistProfiles.Add(receptionist);
			await _context.SaveChangesAsync();
		}

	
	}

}
