using ClinicSystem.Interfaces;
using ClinicSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ClinicSystem.Repositories
{
    public class AuthRepository : IAuthRepository
	{
		private readonly UserManager<AppUser> _userManager;

		public AuthRepository(UserManager<AppUser> userManager)
		{
			_userManager = userManager;
		}

		public async Task AddToRoleAsync(AppUser user, string roleName)
		{
			 await _userManager.AddToRoleAsync(user, roleName);
		}

		public async Task<bool> CheckPasswordAsync(AppUser user, string password)
		{
			return await _userManager.CheckPasswordAsync(user, password);
		}

		public async Task<bool> CreateAsync(AppUser user, string password)
		{
			var result =await _userManager.CreateAsync(user, password);
			if (result.Succeeded) return true;
			return false;
		}

		public async Task<AppUser?> GetByEmailAsync(string email)
		{
		   return  await _userManager.FindByEmailAsync(email);
		}
		public async Task<IList<string>> GetRolesAsync(AppUser user)
		{
			return await _userManager.GetRolesAsync(user);
		}
		public async Task<bool> ResetPasswordAsync(AppUser user, string newPassword)
		{
			var token = await _userManager.GeneratePasswordResetTokenAsync(user);
			var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
			return result.Succeeded;
		}
		public async Task<List<AppUser>> GetUsersByRoleAsync(string role)
		{
			var usersInRole = await _userManager.GetUsersInRoleAsync(role);
			return usersInRole.ToList();
		}
		public async Task<AppUser?> GetByIdAsync(string id)
		{
			return await _userManager.FindByIdAsync(id);
		}
		public async Task<bool> DeleteUserAsync(AppUser user)
		{
			var result = await _userManager.DeleteAsync(user);
			return result.Succeeded;
		}
		public async Task<bool> UpdateSecurityStampAsync(AppUser user)
		{
			var result = await _userManager.UpdateSecurityStampAsync(user);
			return result.Succeeded;
		}
		public async Task<bool> UpdateUserAsync(AppUser user)
		{
			var result = await _userManager.UpdateAsync(user);
			return result.Succeeded;
		}


	}
}
