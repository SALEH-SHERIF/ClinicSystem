
using ClinicSystem.Interfaces;
using ClinicSystem.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ClinicSystem.Services
{
    public class JwtService : IJwtService
	{
		private readonly IConfiguration _configuration;
		public JwtService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public string GenerateToken(AppUser user, IList<string> roles)
		{
			var authClaims = new List<Claim>
			{
		new Claim(ClaimTypes.NameIdentifier, user.Id),
		new Claim(ClaimTypes.Name, user.FullName),
		new Claim(ClaimTypes.Email, user.Email),
		new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
		 };

			foreach (var role in roles)
			{
				authClaims.Add(new Claim(ClaimTypes.Role, role));
			}
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _configuration["JwtSettings:Issuer"],
				audience: _configuration["JwtSettings:Audience"],
				claims: authClaims,
				expires: DateTime.UtcNow.AddDays(7),
				signingCredentials: creds);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
