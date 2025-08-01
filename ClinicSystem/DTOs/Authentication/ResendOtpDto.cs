﻿using System.ComponentModel.DataAnnotations;

namespace ClinicSystem.DTOs
{
	public class ResendOtpDto
	{
		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid email format")]
		public string Email { get; set; } = string.Empty;
	}
}
