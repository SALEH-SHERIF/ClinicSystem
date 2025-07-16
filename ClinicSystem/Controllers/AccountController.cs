using ClinicSystem.DTOs;
using ClinicSystem.DTOs.Authentication;
using ClinicSystem.Interfaces;
using ClinicSystem.Models;
using ClinicSystem.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicSystem.Controllers
{
    [Route("api/[controller]")]
	[ApiController]
	
	public class AccountController : ControllerBase
	{
		private readonly IAuthService _authService;
		public AccountController(IAuthService authService)
		{
			_authService = authService;

		}
		[HttpPost("Register-patient")]
		public async Task<IActionResult> Registerpatient([FromBody] RegisterPatientDto registerPatientDto)
		{
            var result = await _authService.RegisterPatientAsync(registerPatientDto);
             if (!result.Success)
				return BadRequest(result); 
             return Ok(result); 
		}

	
		[HttpPost("resend-registration-otp")]
		public async Task<IActionResult> ResendRegistrationOtp([FromBody] ResendOtpDto dto)
		{
			var response = await _authService.ResendOtpAsync(dto, OtpPurpose.EmailVerification);
			if (!response.Success)
				return BadRequest(response);
			return Ok(response);
		}

		[HttpPost("verify-otp")]
		public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
		{
			var result = await _authService.VerifyOtpAsync(dto);
			if (!result.Success)
				return StatusCode(422, ApiResponse<string>.Failure(result.Message));

			return Ok(ApiResponse<string>.SuccessResponse(result.Message));
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
		{
			var result = await _authService.LoginAsync(loginDto, HttpContext);
			if (!result.Success)
				return BadRequest(result);

			return Ok(result);
		}
		
		[Authorize]
		[HttpGet("check-auth")]
		public IActionResult CheckAuth()
		{
			return Ok("You are authenticated");
		}
		
		[HttpPost("forgot-password")]
		public async Task<IActionResult> ForgotPassword([FromBody] ForgetPasswordDto forgetPasswordDto)
		{
			var result = await _authService.ForgotPasswordAsync(forgetPasswordDto);
			if (!result.Success)
				return BadRequest(result);
			return Ok(result);
		}

		[HttpPost("resend-forgot-password-otp")]
		public async Task<IActionResult> ResendForgotPasswordOtp([FromBody] ResendOtpDto dto)
		{
			var response = await _authService.ResendOtpAsync(dto, OtpPurpose.PasswordReset);
			if (!response.Success)
				return BadRequest(response);
			return Ok(response);
		}

		[HttpPost("verify-reset-otp")]
		public async Task<IActionResult> VerifyResetOtp([FromBody] VerifyOtpDto verifyOtpDto)
		{
			var result = await _authService.VerifyResetOtpAsync(verifyOtpDto);
			if (!result.Success)
				return StatusCode(422, result);

			return Ok(result);
		}
		
		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
		{
			var result = await _authService.ResetPasswordAsync(dto);
			if (!result.Success)
				return BadRequest(result);

			if (Request.Cookies.ContainsKey("jwt"))
			{
				Response.Cookies.Delete("jwt", new CookieOptions
				{
					HttpOnly = true,
					Secure = true,         
					SameSite = SameSiteMode.None,
					Path = "/"
				});
			}
				return Ok(result);
			
		}

		[HttpPost("logout")]
		public IActionResult Logout()
		{
			if (Request.Cookies.ContainsKey("jwt"))
			{
				Response.Cookies.Delete("jwt", new CookieOptions
				{
					HttpOnly = true,
					Secure = true,         
					SameSite = SameSiteMode.None,
					Path = "/"
				});
			}
			return Ok(new ApiResponse<string>(true, "Logged out successfully"));
		}

		[Authorize(Roles = "Doctor")]
		[HttpPost("register-receptionist")]
		public async Task<IActionResult> RegisterReceptionist([FromBody] RegisterReceptionistDto dto)
		{
           var result = await _authService.RegisterReceptionistAsync(dto);
			if (!result.Success)
				return BadRequest(result);
			return Ok(result);
		}

		[Authorize(Roles = "Doctor")]
		[HttpGet("users/by-rolebyDoctor")]
		public async Task<IActionResult> GetUsersByRole( string role)
		{
			var allowedRoles = new[] { "Patient", "Receptionist" };
			if (!allowedRoles.Contains(role))
				return BadRequest(ApiResponse<string>.Failure("Invalid role name."));

			var result = await _authService.GetUsersByRoleAsync(role);
			if (!result.Success)
				return BadRequest(result);
			return Ok(result);
		}

		[Authorize(Roles = "Doctor")]
		[HttpDelete("DeletereceptionistbyDoctor")]
		public async Task<IActionResult> DeleteReceptionist( string id)
		{
			var result = await _authService.DeleteReceptionistAsync(id);
			if (!result.Success)
				return BadRequest(result);

			return Ok(result);
		}

		[Authorize(Roles = "Doctor,Receptionist")]
		[HttpDelete("delete-patientbyDoctor,Receptionist")]
		public async Task<IActionResult> DeletePatient(string id)
		{
			var result = await _authService.DeletePatientAsync(id);
			if (!result.Success)
				return BadRequest(result);

			return Ok(result);
		}
		[HttpPost("change-password")]
		[Authorize]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
		{
			var result = await _authService.ChangePasswordAsync(dto, HttpContext);
			if(result.Success) 
			 return Ok(result);
			return BadRequest(result);
		}


	}
}
