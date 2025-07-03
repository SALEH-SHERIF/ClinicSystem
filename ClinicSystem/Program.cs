using ClinicSystem.Data;
using ClinicSystem.Interfaces;
using ClinicSystem.Models;
using ClinicSystem.Repositories;
using ClinicSystem.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace ClinicSystem
{
    #region DataSeeder
    public static class DataSeeder
	{
		public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
		{
			string[] roleNames = { "Patient", "Doctor", "Receptionist" };

			foreach (var roleName in roleNames)
			{
				if (!await roleManager.RoleExistsAsync(roleName))
					await roleManager.CreateAsync(new IdentityRole(roleName));
			}
		}
	}
	#endregion

	public class SeedUserService
	{
		private readonly IAuthRepository _authRepository;
		private readonly DoctorUserSettings _adminSettings;

		public SeedUserService(IAuthRepository authRepository, IOptions<DoctorUserSettings> adminSettings)
		{
			_authRepository = authRepository;
			_adminSettings = adminSettings.Value;
		}

		#region SeedDoctorUserAsync
		public async Task SeedDoctorUserAsync()
		{
			var user = await _authRepository.GetByEmailAsync(_adminSettings.Email);
			if (user == null)
			{
				var newUser = new AppUser
				{
					Email = _adminSettings.Email,
					UserName = _adminSettings.Email,
					FullName = _adminSettings.FullName,
					EmailConfirmed = true,
					PhoneNumber="+201206558647" ,
					Nationality= "Egyptian" , 
					NationalId="30307082102677",
				};

				var created = await _authRepository.CreateAsync(newUser, _adminSettings.Password);
				if (created)
				{
					await _authRepository.AddToRoleAsync(newUser, "Doctor");
					Console.WriteLine("Doctor user created successfully.");
				}
				else
				{
					Console.WriteLine("Failed to create Doctor user.");
				}
			}
			else
			{
				Console.WriteLine("Doctor user already exists.");
			}
		}

		#endregion
	}

	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			#region Services Injection

			builder.Services.AddScoped<IAuthRepository, AuthRepository>();
			builder.Services.AddScoped<IAuthService, AuthService>();
			builder.Services.AddScoped<IOtpRepository, OtpRepository>();
			builder.Services.AddScoped<IMailService, MailService>();
			builder.Services.AddScoped<IJwtService, JwtService>();
			builder.Services.AddScoped<SeedUserService>();

			#endregion

			#region App Settings & Configuration

			builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

			#endregion
			#region Seed Date 

			builder.Services.Configure<DoctorUserSettings>(builder.Configuration.GetSection("DoctorUser"));


			#endregion

			#region DbContext

			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

			#endregion

			#region Identity

			builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
			{
				options.User.RequireUniqueEmail = true;
			})
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddDefaultTokenProviders();

			#endregion

			#region JWT Authentication with Cookie support

			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.Events = new JwtBearerEvents
				{
					OnMessageReceived = context =>
					{
						// ????? ?????? ?? ??????? ???? "jwt"
						if (context.Request.Cookies.ContainsKey("jwt"))
							context.Token = context.Request.Cookies["jwt"];

						return Task.CompletedTask;
					}
				};

				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
					ValidAudience = builder.Configuration["JwtSettings:Audience"],
					IssuerSigningKey = new SymmetricSecurityKey(
						Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"])
					)
				};
			});

			#endregion

			#region CORS

			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowFrontend", policy =>
				{
					policy.WithOrigins("http://localhost:8080") // ????? ??????? ????? ??? ??????
						  .AllowAnyHeader()
						  .AllowAnyMethod()
						  .AllowCredentials(); // ??? ???? ?????? ?????? ???????
				});
			});

			#endregion

			#region ModelState Custom Response

			builder.Services.AddControllers()
				.ConfigureApiBehaviorOptions(options =>
				{
					options.InvalidModelStateResponseFactory = actionContext =>
					{
						var errors = actionContext.ModelState
							.Where(m => m.Value?.Errors.Count > 0)
							.SelectMany(m => m.Value.Errors)
							.Select(e => e.ErrorMessage)
							.ToList();

						var errorResponse = new ApiResponse<List<string>>
						{
							Success = false,
							Message = "Validation failed.",
							Data = errors
						};

						return new BadRequestObjectResult(errorResponse);
					};
				});

			#endregion

			#region Swagger Configuration

			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(swagger =>
			{
				swagger.SwaggerDoc("v1", new OpenApiInfo
				{
					Version = "v1",
					Title = "Clinic System API",
					Description = "ClinicSystem"
				});

				swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Name = "Authorization",
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer",
					BearerFormat = "JWT",
					In = ParameterLocation.Header,
					Description = "???? 'Bearer ' ?? ?????? ????? ??"
				});

				swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							}
						},
						new List<string>()
					}
				});
			});

			#endregion

			var app = builder.Build();

			#region Seed Roles on Startup & SeedDoctorUser

			using (var scope = app.Services.CreateScope())
			{
				var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
				await DataSeeder.SeedRolesAsync(roleManager);
			}
			using (var scope = app.Services.CreateScope())
			{
				try
				{
					var seedUserService = scope.ServiceProvider.GetRequiredService<SeedUserService>();
					await seedUserService.SeedDoctorUserAsync();
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error seeding admin user: " + ex.Message);
					throw;
				}
			}


			#endregion

			app.UseCookiePolicy(new CookiePolicyOptions
			{
				CheckConsentNeeded = _ => false,
				MinimumSameSitePolicy = SameSiteMode.None // ???? None ???? ??????? ????? ?? CORS
			});

			#region Middleware Pipeline

			app.UseSwagger();
			app.UseSwaggerUI();

			app.UseHttpsRedirection();

			app.UseCors("AllowFrontend");

			app.UseAuthentication();
			app.UseAuthorization();
			app.MapControllers();

			#endregion

			app.Run();
		}
	}
}
