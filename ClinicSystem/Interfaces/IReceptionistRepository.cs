using ClinicSystem.Models;

namespace ClinicSystem.Interfaces
{
	public interface IReceptionistRepository
	{
		Task CreateAsync(ReceptionistProfile receptionist);
	}
}
