using ClinicSystem.DTOs;
using ClinicSystem.DTOs.Authentication;
using ClinicSystem.Interfaces;
using ClinicSystem.Models;

using ClinicSystem.Models.Enums;
using ClinicSystem.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace ClinicSystem.Services
{
    public class AuthService : IAuthService
	{
		private readonly IAuthRepository _authRepository;
		private readonly IJwtService _jwtService;
		private readonly IOtpRepository _otpRepository;
		private readonly IMailService _mailService;
		private readonly IPatientRepository _patientRepository;
		private readonly IReceptionistRepository _receptionistRepository;



		public AuthService(IAuthRepository authRepo, IOtpRepository otpRepository , IMailService mailService, IJwtService jwtService, IPatientRepository patientRepository, IReceptionistRepository receptionistRepository)
		{
			_authRepository = authRepo;
			_otpRepository = otpRepository;
			_mailService = mailService;
			_jwtService = jwtService;
			_patientRepository = patientRepository;
			_receptionistRepository = receptionistRepository;
		}

		public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto loginDto, HttpContext httpContext)
		{
			try
			{
				var user = await _authRepository.GetByEmailAsync(loginDto.Email);
				if (user == null)
					return ApiResponse<LoginResponseDto>.Failure("Invalid email or password");

				if (!user.EmailConfirmed)
					return ApiResponse<LoginResponseDto>.Failure("Email is not confirmed. Please verify your email.");
				
				var roles = await _authRepository.GetRolesAsync(user);
				var role = roles.FirstOrDefault() ?? "Unknown";

				var passwordValid = await _authRepository.CheckPasswordAsync(user, loginDto.Password);
				if (!passwordValid)
					return ApiResponse<LoginResponseDto>.Failure("Invalid email or password");

				var token = _jwtService.GenerateToken(user , roles);
				
				// Store token in HttpOnly cookie
				httpContext.Response.Cookies.Append("jwt", token, new CookieOptions
				{
					HttpOnly = true,
					Secure = true, // set to false in localhost if needed
					SameSite = SameSiteMode.None,
					Expires = DateTimeOffset.UtcNow.AddDays(7)
				});

				var response = new LoginResponseDto
				{
					Email = user.Email,
					FullName = user.FullName,
					Role = role
				};

				return ApiResponse<LoginResponseDto>.SuccessResponse("Login successful", response);
			}
			catch (Exception ex)
			{
				var inner = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
				return ApiResponse<LoginResponseDto>.Failure("Unexpected error: " + inner);
			}
		}

		public async Task<ApiResponse<string>> RegisterPatientAsync(RegisterPatientDto registerPatientDto)
		{
			try
			{
				var exists = await _authRepository.GetByEmailAsync(registerPatientDto.Email);
				if (exists != null)
				{
					if (!exists.EmailConfirmed)
					{
						exists.FullNameArabic = registerPatientDto.ArabicFullName;
						exists.FullName = registerPatientDto.EnglishFullName;
                        exists.PhoneNumber = registerPatientDto.PhoneNumber;
						exists.Nationality = registerPatientDto.Nationality;
						exists.NationalId = registerPatientDto.Nationality == "Egyptian" ? registerPatientDto.NationalId : null;
						exists.PassportNumber = registerPatientDto.Nationality == "Foreigner" ? registerPatientDto.PassportNumber : null;
						exists.BirthDate = registerPatientDto.BirthDate; 

						await _authRepository.UpdateUserAsync(exists);


						var newOtp = await _otpRepository.ResendOtpAsync(exists , OtpPurpose.EmailVerification);
						if (newOtp == null)
							return ApiResponse<string>.Failure("Failed to generate new OTP.");

						var resendMsg = new MailRequest
						{
							Subject = "Your New OTP Code",
							Body = $"<p>Your new OTP code is: <strong>{newOtp.Code}</strong></p><p>This code is valid for 5 minutes.</p>",
							ToEmail = exists.Email
						};
						await _mailService.SendEmailAsync(resendMsg);

						return ApiResponse<string>.SuccessResponse("Your account is not yet confirmed. A new OTP has been sent to your email.");

					}
					else
					{
						return ApiResponse<string>.Failure("Email already exists");
					}
				}
			
				var user = new AppUser
				{
					FullName = registerPatientDto.EnglishFullName,
					FullNameArabic=registerPatientDto.ArabicFullName,
					Email = registerPatientDto.Email,
					UserName = registerPatientDto.Email,
					PhoneNumber = registerPatientDto.PhoneNumber,
					Nationality = registerPatientDto.Nationality,
					NationalId = registerPatientDto.Nationality == "Egyptian" ? registerPatientDto.NationalId : null,
					PassportNumber = registerPatientDto.Nationality == "Foreigner" ? registerPatientDto.PassportNumber : null,
					BirthDate = registerPatientDto.BirthDate,
					EmailConfirmed = false
				};
				var created = await _authRepository.CreateAsync(user, registerPatientDto.Password);
				if (!created)
					return ApiResponse<string>.Failure("User creation failed");

				await _authRepository.AddToRoleAsync(user, "Patient");
				var patientProfile = new PatientProfile
				{
					AppUserId = user.Id,
				};

				await _patientRepository.CreateAsync(patientProfile);

				var otp = await _otpRepository.GenerateAndStoreOtpAsync(user ,OtpPurpose.EmailVerification);

				if(otp ==null)
					return ApiResponse<string>.Failure($"Failed to generate or store otp");

				var msg = new MailRequest
				{
					Subject = "Your OTP Is On Its Way!",
					Body = $@"
               <p>Hello {registerPatientDto.EnglishFullName},</p>
              <p>We noticed that you requested an OTP again. Don't worry, your previous code is still valid!</p>
              <p>Here is your One-Time Password (OTP):</p>
              <h1 style='color: #00bfff;'>{otp.Code}</h1>
              <p><strong>Note:</strong> This code is valid for the next <strong>5 minutes</strong>.</p>
               <p>If you did not request this, please ignore this email. Your account is secure.</p>
                <p>Take care,</p>
              <p><strong>The ClinicSystem Team</strong></p>",
					ToEmail = registerPatientDto.Email
				};
				  await _mailService.SendEmailAsync(msg);

				return ApiResponse<string>.SuccessResponse("Registered successfully");
			}
			catch (Exception ex)
			{
				var inner = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
				return ApiResponse<string>.Failure("Unexpected error: " + inner);
			}
		}

		public async Task<ApiResponse<string>> ResendOtpAsync(ResendOtpDto dto , OtpPurpose purpose)
		{
			var user = await _authRepository.GetByEmailAsync(dto.Email);
			if (user == null)
				return ApiResponse<string>.Failure("User not found");
			
			if (purpose == OtpPurpose.EmailVerification)
			{
				if (user.EmailConfirmed)
					return ApiResponse<string>.Failure("Email is already confirmed");
			}


			var newOtp = await _otpRepository.ResendOtpAsync(user , purpose);
			if (newOtp == null)
				return ApiResponse<string>.Failure("Invalid Send Otp");
			var msg = new MailRequest
			{
				Subject = "Your OTP Code",
				Body = $"<p>Your OTP code is: <strong>{newOtp.Code}</strong></p><p>This code is valid for 5 minutes.</p>",
				ToEmail = user.Email
			};

			await _mailService.SendEmailAsync(msg);

			return ApiResponse<string>.SuccessResponse("OTP code resent successfully");
		}

		public async Task<ApiResponse<string>> ForgotPasswordAsync([FromBody] ForgetPasswordDto forgetPasswordDto)
		{
			var user = await _authRepository.GetByEmailAsync(forgetPasswordDto.Email);
			if (user == null)
				return ApiResponse<string>.Failure("User not found");

			if (!user.EmailConfirmed)
				return ApiResponse<string>.Failure("Email is not verified");

			var otp = await _otpRepository.GenerateAndStoreOtpAsync(user, OtpPurpose.PasswordReset);
			if (otp == null)
				return ApiResponse<string>.Failure("Failed to generate OTP");

			var msg = new MailRequest
			{
				Subject = "Reset Your Password - OTP Code Inside!",
				Body = $@"
	<p>Hello {user.FullName},</p>
	<p>We received a request to reset your password for your ClinicSystem account.</p>
	<p>Your One-Time Password (OTP) is:</p>
	<h1 style='color: #e74c3c;'>{otp.Code}</h1>
	<p>This code is valid for the next <strong>5 minutes</strong>.</p>
	<p>If you did not request this, you can safely ignore this email.</p>
	<p>Stay safe,</p>
	<p><strong>The ClinicSystem Team</strong></p>
	",
				ToEmail = forgetPasswordDto.Email
			};

			await _mailService.SendEmailAsync(msg);


			return ApiResponse<string>.SuccessResponse("OTP sent to your email.");
		}

		public async Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto1)
		{
			var user = await _authRepository.GetByEmailAsync(resetPasswordDto1.Email);
			if (user == null)
				return ApiResponse<string>.Failure("User not found");

			var otp = await _otpRepository.GetValidOtpAsync(user, resetPasswordDto1.OtpCode, OtpPurpose.PasswordReset);
			if (otp == null)
				return ApiResponse<string>.Failure("Invalid or expired OTP");

			var result = await _authRepository.ResetPasswordAsync(user, resetPasswordDto1.Password);
			if (!result)
				return ApiResponse<string>.Failure("Failed to reset password");

			await _otpRepository.MarkOtpAsUsedAsync(otp);
			await _authRepository.UpdateSecurityStampAsync(user);


			return ApiResponse<string>.SuccessResponse("Password has been reset successfully. Please login again.");
		}

		public async Task<ApiResponse<string>> VerifyOtpAsync(VerifyOtpDto dto)
		{
				var user = await _authRepository.GetByEmailAsync(dto.Email);
				if (user == null)
					return ApiResponse<string>.Failure("User not found");

				var otp = await _otpRepository.GetValidOtpAsync(user, dto.Code , OtpPurpose.EmailVerification);
				if (otp == null)
					return ApiResponse<string>.Failure("Invalid or expired OTP");

				user.EmailConfirmed = true;
				await _otpRepository.MarkOtpAsUsedAsync(otp);

				return ApiResponse<string>.SuccessResponse("OTP verified. Account activated.");
		}

		public async Task<ApiResponse<string>> VerifyResetOtpAsync(VerifyOtpDto verifyOtpDto)
		{
			var user = await _authRepository.GetByEmailAsync(verifyOtpDto.Email);
			if (user == null)
				return ApiResponse<string>.Failure("User not found");

			var otp = await _otpRepository.GetValidOtpAsync(user, verifyOtpDto.Code, OtpPurpose.PasswordReset);
			if (otp == null)
				return ApiResponse<string>.Failure("Invalid or expired OTP");

			// await _otpRepository.MarkOtpAsUsedAsync(otp);

			return ApiResponse<string>.SuccessResponse("OTP verified. You can now reset your password.");
		}

		public async Task<ApiResponse<string>> RegisterReceptionistAsync(RegisterReceptionistDto dto)
		{
			try
			{
				var exists = await _authRepository.GetByEmailAsync(dto.Email);
				if (exists != null)
					return ApiResponse<string>.Failure("Email already exists");

				var receptionist = new AppUser
				{
					FullName = dto.FullName,
					Email = dto.Email,
					UserName = dto.Email,
					PhoneNumber = dto.PhoneNumber,
					EmailConfirmed = true 
				};

				var created = await _authRepository.CreateAsync(receptionist, dto.Password);
				if (!created)
					return ApiResponse<string>.Failure("Failed to create receptionist account");

				await _authRepository.AddToRoleAsync(receptionist, "Receptionist");
				await _receptionistRepository.CreateAsync(new ReceptionistProfile
				{
					AppUserId = receptionist.Id,
					
				});

				return ApiResponse<string>.SuccessResponse("Receptionist added successfully");
			}
			catch (Exception ex)
			{
				var inner = ex.InnerException?.Message ?? ex.Message;
				return ApiResponse<string>.Failure("Unexpected error: " + inner);
			}
		}
		public async Task<ApiResponse<List<UserSummaryDto>>> GetUsersByRoleAsync(string role)
		{
			try
			{
				var users = await _authRepository.GetUsersByRoleAsync(role);
				if (users == null || users.Count == 0)
					return ApiResponse<List<UserSummaryDto>>.Failure($"No users found with role '{role}'");


				var result = users.Select(u => new UserSummaryDto
				{
					Id = u.Id,
					FullName = u.FullName,
					Email = u.Email,
					PhoneNumber = u.PhoneNumber,
				}).ToList();

				return ApiResponse<List<UserSummaryDto>>.SuccessResponse($"Users with role '{role}' fetched successfully", result);
			}
			catch (Exception ex)
			{
				return ApiResponse<List<UserSummaryDto>>.Failure("Error: " + ex.Message);
			}
		}

		public async Task<ApiResponse<string>> DeleteReceptionistAsync(string id)
		{
			try
			{
				var user = await _authRepository.GetByIdAsync(id);
				if (user == null)
					return ApiResponse<string>.Failure("User not found");

				var roles = await _authRepository.GetRolesAsync(user);
				if (!roles.Contains("Receptionist"))
					return ApiResponse<string>.Failure("User is not a receptionist");

				var deleted = await _authRepository.DeleteUserAsync(user);
				if (!deleted)
					return ApiResponse<string>.Failure("Failed to delete user");

				return ApiResponse<string>.SuccessResponse("Receptionist deleted successfully");
			}
			catch (Exception ex)
			{
				var error = ex.InnerException?.Message ?? ex.Message;
				return ApiResponse<string>.Failure("Unexpected error occurred: " + error);
			}
		}
		public async Task<ApiResponse<string>> DeletePatientAsync(string id)
		{
			try
			{
				var user = await _authRepository.GetByIdAsync(id);
				if (user == null)
					return ApiResponse<string>.Failure("User not found");

				var roles = await _authRepository.GetRolesAsync(user);
				if (!roles.Contains("Patient"))
					return ApiResponse<string>.Failure("User is not a patient");

				var deleted = await _authRepository.DeleteUserAsync(user);
				if (!deleted)
					return ApiResponse<string>.Failure("Failed to delete patient");

				return ApiResponse<string>.SuccessResponse("Patient deleted successfully");
			}
			catch (Exception ex)
			{
				var error = ex.InnerException?.Message ?? ex.Message;
				return ApiResponse<string>.Failure("Unexpected error: " + error);
			}
		}

		public async Task<ApiResponse<string>> ChangePasswordAsync(ChangePasswordDto model, HttpContext httpContext)
		{
			var userId = _jwtService.GetUserIdFromToken(httpContext);
			if (string.IsNullOrEmpty(userId))
				return ApiResponse<string>.Failure("Unauthorized");

			var user = await _authRepository.GetByIdAsync(userId);
			if (user == null)
				return ApiResponse<string>.Failure("User not found");

			var isOldPasswordValid = await _authRepository.CheckPasswordAsync(user, model.OldPassword);
			if (!isOldPasswordValid)
				return ApiResponse<string>.Failure("Old password is incorrect");

			var result = await _authRepository.ChangePasswordAsync(user,  model.NewPassword);
			if (!result.Succeeded)
				return ApiResponse<string>.Failure("Failed to change password");

			
			await _authRepository.UpdateSecurityStampAsync(user);

			
			httpContext.Response.Cookies.Delete("jwt");

			return ApiResponse<string>.SuccessResponse("Password changed successfully. Please log in again.");
		}



	}
}
