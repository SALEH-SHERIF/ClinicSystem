using ClinicSystem.Models;

namespace ClinicSystem.Interfaces
{
	public interface IPatientRepository
	{
		Task CreateAsync(PatientProfile patient);
		Task UpdateAsync(PatientProfile patient);
	}
}
