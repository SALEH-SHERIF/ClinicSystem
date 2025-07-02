using ClinicSystem.DTOs;
using ClinicSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace ClinicSystem.Interfaces
{
    public interface IAuthRepository
    {
        Task<AppUser?> GetByEmailAsync(string email);
        Task<bool> CreateAsync(AppUser user, string password);
        Task AddToRoleAsync(AppUser user, string roleName);
		Task<bool> CheckPasswordAsync(AppUser user, string password);
		Task<IList<string>> GetRolesAsync(AppUser user);
		Task<bool> ResetPasswordAsync(AppUser user, string newPassword);
		Task<List<AppUser>> GetUsersByRoleAsync(string role);
		Task<AppUser?> GetByIdAsync(string id);
		Task<bool> DeleteUserAsync(AppUser user);



	}
}
