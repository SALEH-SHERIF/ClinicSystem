using ClinicSystem.Models;
using ClinicSystem.Models.Enums;

namespace ClinicSystem.Interfaces
{
    public interface IOtpRepository
    {
        Task<UserOtp> GenerateAndStoreOtpAsync(AppUser user, OtpPurpose purpose);
		Task<UserOtp?> GetValidOtpAsync(AppUser user, string code , OtpPurpose purpose);
		Task MarkOtpAsUsedAsync(UserOtp otp);
		Task<UserOtp> ResendOtpAsync(AppUser user , OtpPurpose purpose);
		

	}
}
