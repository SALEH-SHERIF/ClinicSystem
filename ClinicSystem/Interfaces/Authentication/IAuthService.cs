using ClinicSystem.DTOs;
using ClinicSystem.DTOs.Authentication;
using ClinicSystem.Models;
using ClinicSystem.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ClinicSystem.Interfaces
{
    public interface IAuthService
	{
		Task<ApiResponse<string>> RegisterPatientAsync(RegisterPatientDto registerPatientDto);
		Task<ApiResponse<string>> VerifyOtpAsync(VerifyOtpDto dto);
		Task<ApiResponse<string>> ResendOtpAsync(ResendOtpDto dto  , OtpPurpose purpose);
		Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto loginDto , HttpContext httpContext);
		Task<ApiResponse<string>> ForgotPasswordAsync(ForgetPasswordDto forgetPasswordDto);
		Task<ApiResponse<string>> VerifyResetOtpAsync(VerifyOtpDto verifyOtpDto);
		Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);

		//Receptionist
		Task<ApiResponse<string>> RegisterReceptionistAsync(RegisterReceptionistDto dto);
		Task<ApiResponse<List<UserSummaryDto>>> GetUsersByRoleAsync(string role);
        Task<ApiResponse<string>> DeleteReceptionistAsync(string id);
		Task<ApiResponse<string>> DeletePatientAsync(string patientId);


	}
}
