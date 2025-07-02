using ClinicSystem.Data;
using ClinicSystem.Interfaces;
using ClinicSystem.Models;
using ClinicSystem.Models.Enums;
using ClinicSystem.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace ClinicSystem.Repositories
{
    public class OtpRepository : IOtpRepository
	{
		private readonly ApplicationDbContext _dbContext;

		public OtpRepository(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<UserOtp> GenerateAndStoreOtpAsync(AppUser user, OtpPurpose purpose)
		{
			var existingOtps = _dbContext.UserOtps.Where(o => o.UserId == user.Id && o.Purpose == purpose);
			_dbContext.UserOtps.RemoveRange(existingOtps);

			var otpCode = new Random().Next(100000,999999).ToString();
			var otp = new UserOtp
			{
				Code = otpCode , 
				ExpirationTime = DateTime.UtcNow.AddMinutes(5),
				UserId = user.Id ,
				Purpose = purpose
		    };
			 _dbContext.UserOtps.Add(otp);
            await _dbContext.SaveChangesAsync();
			return otp;
		}
		public async Task<UserOtp?> GetValidOtpAsync(AppUser user, string code  , OtpPurpose purpose)
		{
			return await _dbContext.UserOtps
				.Where(o => o.UserId == user.Id && o.Code == code && !o.IsUsed && o.ExpirationTime > DateTime.UtcNow && o.Purpose == purpose)
				.OrderByDescending(o => o.ExpirationTime)
				.FirstOrDefaultAsync();
		}

		public async Task MarkOtpAsUsedAsync(UserOtp otp)
		{
			otp.IsUsed = true;
			await _dbContext.SaveChangesAsync();
		}

		public async Task<UserOtp> ResendOtpAsync(AppUser user , OtpPurpose purpose)
		{
			var existingOtp = await _dbContext.UserOtps
			.Where(o => o.UserId == user.Id && !o.IsUsed && o.ExpirationTime > DateTime.UtcNow && o.Purpose == purpose)
			.OrderByDescending(o => o.ExpirationTime)
			.FirstOrDefaultAsync();

			if (existingOtp != null)
				return existingOtp;

			// generate new one
			return await GenerateAndStoreOtpAsync(user, purpose);
		}
	}
}
