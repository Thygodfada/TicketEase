using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TicketEase.Application.DTO;
using TicketEase.Application.DTO.Manager;
using TicketEase.Application.Interfaces.Repositories;
using TicketEase.Application.Interfaces.Services;
using TicketEase.Common.Utilities;
using TicketEase.Domain;
using TicketEase.Domain.Entities;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace TicketEase.Application.ServicesImplementation
{
	public class AuthenticationService : IAuthenticationService
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly EmailServices _emailServices;
		private readonly EmailSettings _emailSettings;
		private readonly ILogger _logger;
		private readonly IConfiguration _config;


		public AuthenticationService(IConfiguration config, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IOptions<EmailSettings> emailSettings, ILogger<AuthenticationService> logger)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_emailServices = new EmailServices(emailSettings);
			_emailSettings = emailSettings.Value;
			_logger = logger;
			_config = config;
		}

		public async Task<ApiResponse<string>> RegisterAsync(AppUserCreateDto appUserCreateDto)
		{
			var user = await _userManager.FindByEmailAsync(appUserCreateDto.Email);
			if (user != null)
			{
				return new ApiResponse<string>(false, "User with this email already exist.", StatusCodes.Status400BadRequest, new List<string>());
			}
			var appUser = new AppUser()
			{
				FirstName = appUserCreateDto.FirstName,
				LastName = appUserCreateDto.LastName,
				Email = appUserCreateDto.Email,
				ManagerId = appUserCreateDto.ManagerId,
				UserName = appUserCreateDto.Email,
			};
			
			try
			{
				var result = await _userManager.CreateAsync(appUser, appUserCreateDto.Password);
				if (result.Succeeded)
				{
					await _userManager.AddToRoleAsync(appUser, "SuperAdmin");
				}
				return new ApiResponse<string>(true, StatusCodes.Status201Created, "User registered successfully");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while adding a manager " + ex.InnerException);
				var errorList = new List<string>();
				errorList.Add(ex.InnerException.ToString());
				return new ApiResponse<string>(false, "Error creating user.", StatusCodes.Status500InternalServerError, null, errorList);
			}
		}

       

        public async Task<ApiResponse<string>> RegisterManagerAsync(AppUserCreateDto appUserCreateDto)
		{
			try
			{
				var user = await _userManager.FindByEmailAsync(appUserCreateDto.Email);
				if (user != null)
				{
					return new ApiResponse<string>(false, StatusCodes.Status400BadRequest, "Manager with this email already exist.");
				}
				var appUser = new AppUser()
				{
					Email = appUserCreateDto.Email,
					NormalizedEmail = appUserCreateDto.Email,
					ManagerId = appUserCreateDto.ManagerId,
					UserName = appUserCreateDto.Email,
					IsActive = true,
					EmailConfirmed = true,
					LockoutEnabled = false,
					NormalizedUserName = appUserCreateDto.Email					
				};
				var result = await _userManager.CreateAsync(appUser, appUserCreateDto.Password);
				if (result.Succeeded)
				{
					await _userManager.AddToRoleAsync(appUser, "Manager");
				}
				return new ApiResponse<string>(true, StatusCodes.Status201Created, "Manager registered successfully.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while adding a manager " + ex.InnerException);
				return new ApiResponse<string>(false, "Error occurred while creating manager.", StatusCodes.Status500InternalServerError, new List<string> { ex.InnerException.ToString() });
			}
		}

		public async Task<ApiResponse<string>> LoginAsync(AppUserLoginDto loginDTO)
		{
			try
			{
				var user = await _userManager.FindByEmailAsync(loginDTO.Email);
				if (user == null)
				{
					return new ApiResponse<string>(false, StatusCodes.Status404NotFound, "User not found.");
				}
				var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, lockoutOnFailure: false);

				switch (result)
				{
					case { Succeeded: true }:
						var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
						return new ApiResponse<string>(true, StatusCodes.Status200OK, GenerateJwtToken(user, role));

					case { IsLockedOut: true }:
						return new ApiResponse<string>(false, StatusCodes.Status403Forbidden, $"Account is locked out. Please try again later or contact support." +
							$" You can unlock your account after {_userManager.Options.Lockout.DefaultLockoutTimeSpan.TotalMinutes} minutes.");

					case { RequiresTwoFactor: true }:
						return new ApiResponse<string>(false, StatusCodes.Status401Unauthorized, "Two-factor authentication is required.");

					case { IsNotAllowed: true }:
						return new ApiResponse<string>(false, StatusCodes.Status401Unauthorized, "Login failed. Email confirmation is required.");

					default:
						return new ApiResponse<string>(false, StatusCodes.Status401Unauthorized, "Login failed. Invalid email or password.");
				}
			}
			catch (Exception ex)
			{
				return new ApiResponse<string>(false, StatusCodes.Status500InternalServerError, "Some error occurred while loggin in." + ex.InnerException);
			}
		}

		public async Task<ApiResponse<string>> ConfirmEmailAsync(string email, string token)
		{
			var user = await _userManager.FindByEmailAsync(email);

			if (user == null)
			{
				return new ApiResponse<string>(false, StatusCodes.Status404NotFound, "User not found.");
			}

			var result = await _userManager.ConfirmEmailAsync(user, token);

			if (result.Succeeded)
			{
				return new ApiResponse<string>(true, StatusCodes.Status200OK, "Email confirmation successful.");
			}

			// Handle confirmation failure
			return new ApiResponse<string>(false, StatusCodes.Status400BadRequest, "Email confirmation failed.");
		}


		private string GenerateJwtToken(AppUser contact, string roles)
		{
			var jwtSettings = _config.GetSection("JwtSettings:Secret").Value;
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings));
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, contact.UserName),
				new Claim(JwtRegisteredClaimNames.Email, contact.Email),
				new Claim(JwtRegisteredClaimNames.Jti, contact.ManagerId),
                new Claim(JwtRegisteredClaimNames.NameId, contact.Id),
                new Claim(ClaimTypes.Role, roles)
			};

			var token = new JwtSecurityToken(
                //issuer: _config.GetValue<string>("JwtSettings:ValidIssuer"),
                //audience: _config.GetValue<string>("JwtSettings:ValidAudience"),
                issuer: null,
				audience: null,
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(int.Parse(_config.GetSection("JwtSettings:AccessTokenExpiration").Value)),
				signingCredentials: credentials
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public async Task<ApiResponse<string>> ForgotPasswordAsync(string email)
		{
			try
			{
				var user = await _userManager.FindByEmailAsync(email);

				if (user == null)
				{
					return new ApiResponse<string>(false, "User not found or email not confirmed.", StatusCodes.Status404NotFound, null, new List<string>());
				}
				string token = await _userManager.GeneratePasswordResetTokenAsync(user);

				user.PasswordResetToken = token;
				user.ResetTokenExpires = DateTime.UtcNow.AddHours(24);

				await _userManager.UpdateAsync(user);

				var resetPasswordUrl = "http://localhost:3000/reset-password?email=" + Uri.EscapeDataString(email) + "&token=" + Uri.EscapeDataString(token);

				var mailRequest = new MailRequest
				{
					ToEmail = email,
					Subject = "TicketEase Password Reset Instructions",
					Body = $"Please reset your password by clicking <a href='{resetPasswordUrl}'>here</a>."
				};
				await _emailServices.SendHtmlEmailAsync(mailRequest);

				return new ApiResponse<string>(true, "Password reset email sent successfully.", 200, null, new List<string>());
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while resolving password change");
				var errorList = new List<string>();
				errorList.Add(ex.Message);
				return new ApiResponse<string>(true, "Error occurred while resolving password change", 500, null, errorList);
			}
		}

		public async Task<ApiResponse<string>> ResetPasswordAsync(string email, string token, string newPassword)
		{
			try
			{
				var user = await _userManager.FindByEmailAsync(email);

				if (user == null)
				{
					return new ApiResponse<string>(false, "User not found.", 404, null, new List<string>());
				}
				var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

				if (result.Succeeded)
				{
					user.PasswordResetToken = null;
					user.ResetTokenExpires = null;

					await _userManager.UpdateAsync(user);

					return new ApiResponse<string>(true, "Password reset successful.", 200, null, new List<string>());
				}
				else
				{
					return new ApiResponse<string>(false, "Password reset failed.", 400, null, result.Errors.Select(error => error.Description).ToList());
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while resetting password");
				var errorList = new List<string> { ex.Message };
				return new ApiResponse<string>(true, "Error occurred while resetting password", 500, null, errorList);
			}
		}

        public async Task<ApiResponse<string>> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_config.GetSection("JwtSettings:Secret").Value);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = null,
                    ValidAudience = null,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

                var emailClaim = principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

                return new ApiResponse<string>(true, "Token is valid.", 200, null, new List<string>());
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogError(ex, "Token validation failed");
                var errorList = new List<string> { ex.Message };
                return new ApiResponse<string>(false, "Token validation failed.", 400, null, errorList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during token validation");
                var errorList = new List<string> { ex.Message };
                return new ApiResponse<string>(false, "Error occurred during token validation", 500, null, errorList);
            }
        }

        public ApiResponse<string> ExtractUserIdFromToken(string authToken)
        {
            try
            {
                var token = authToken.Replace("Bearer ", "");

                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                var userId = jsonToken?.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub)?.Value;

                if (string.IsNullOrWhiteSpace(userId))
                {
                    return new ApiResponse<string>(false, "Invalid or expired token.", 401, null, new List<string>());
                }

                return new ApiResponse<string>(true, "User ID extracted successfully.", 200, userId, new List<string>());
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(false, "Error extracting user ID from token.", 500, null, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<string>> ChangePasswordAsync(AppUser user, string currentPassword, string newPassword)
		{
			try
			{
				var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

				if (result.Succeeded)
				{
					return new ApiResponse<string>(true, "Password changed successfully.", 200, null, new List<string>());
				}
				else
				{
					return new ApiResponse<string>(false, "Password change failed.", 400, null, result.Errors.Select(error => error.Description).ToList());
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while changing password");
				var errorList = new List<string> { ex.Message };
				return new ApiResponse<string>(true, "Error occurred while changing password", 500, null, errorList);
			}
		}
	}
}
