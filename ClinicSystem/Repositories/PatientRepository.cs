using ClinicSystem.Data;
using ClinicSystem.Interfaces;
using ClinicSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicSystem.Repositories
{
	// PatientRepository.cs
	public class PatientRepository : IPatientRepository
	{
		private readonly ApplicationDbContext _context;

		public PatientRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task CreateAsync(PatientProfile patient)
		{
			await _context.PatientProfiles.AddAsync(patient);
			await _context.SaveChangesAsync();
		}


		public async Task UpdateAsync(PatientProfile patient)
		{
			_context.PatientProfiles.Update(patient);
			await _context.SaveChangesAsync();
		}
	}

}
