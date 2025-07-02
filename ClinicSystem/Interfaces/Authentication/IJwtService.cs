using ClinicSystem.Models;

namespace ClinicSystem.Interfaces
{
	public interface IJwtService
	{
		string GenerateToken(AppUser user , IList<string> roles);
	}
}
