using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketEase.Application.DTO;
using TicketEase.Domain;
using TicketEase.Domain.Entities;

namespace TicketEase.Controllers
{
    [Route("api/[controller]")]
	[ApiController]
	public class AuthenticationController : ControllerBase
	{
        private readonly Application.Interfaces.Services.IAuthenticationService _authenticationService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AuthenticationController(Application.Interfaces.Services.IAuthenticationService authenticationService, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _authenticationService = authenticationService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("Register")]
		public async Task<IActionResult> Register([FromBody] AppUserCreateDto appUserCreateDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new ApiResponse<string>(false, "Invalid model state.", StatusCodes.Status400BadRequest, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList()));
			}
			return Ok(await _authenticationService.RegisterAsync(appUserCreateDto));
		}

		[HttpPost("Login")]
		public async Task<IActionResult> Login(AppUserLoginDto loginDTO)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new ApiResponse<string>(false, "Invalid model state.", 400, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList()));
			}
			return Ok(await _authenticationService.LoginAsync(loginDTO));
		}


		[HttpPost("forgot-password")]
		public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new ApiResponse<string>(false, "Invalid model state.", 400, null, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList()));
			}

			var response = await _authenticationService.ForgotPasswordAsync(model.Email);

			if (response.Succeeded)
			{
				return Ok(new ApiResponse<string>(true, response.Message, response.StatusCode, null, new List<string>()));
			}
			else
			{
				return BadRequest(new ApiResponse<string>(false, response.Message, response.StatusCode, null, response.Errors));
			}
		}

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponse<string>(false, "Invalid model state.", 400, null, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList()));
            }

			var response = await _authenticationService.ResetPasswordAsync(model.Email, model.Token, model.NewPassword);

			if (response.Succeeded)
			{
				return Ok(new ApiResponse<string>(true, response.Message, response.StatusCode, null, new List<string>()));
			}
			else
			{
				return BadRequest(new ApiResponse<string>(false, response.Message, response.StatusCode, null, response.Errors));
			}
		}

        [HttpPost("validate-token")]
        public async Task<IActionResult> ValidateToken([FromBody] ValidateTokenDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>(false, "Invalid model state.", 400, null, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList()));
            }

            var response = await _authenticationService.ValidateTokenAsync(model.Token);

            if (response.Succeeded)
            {
                return Ok(new ApiResponse<string>(true, response.Message, response.StatusCode, null, new List<string>()));
            }
            else
            {
                return BadRequest(new ApiResponse<string>(false, response.Message, response.StatusCode, null, response.Errors));
            }
        }

        [HttpPost("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto model, [FromHeader(Name = "Authorization")] string authToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>(false, "Invalid model state.", 400, null, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList()));
            }

            if (string.IsNullOrWhiteSpace(authToken))
            {
                return Unauthorized(new ApiResponse<string>(false, "Authorization token is missing.", 401, null, new List<string>()));
            }

            var userIdResponse = _authenticationService.ExtractUserIdFromToken(authToken);

            if (!userIdResponse.Succeeded)
            {
                return Unauthorized(userIdResponse);
            }

            var userId = userIdResponse.Data;

            var user = await _userManager.FindByEmailAsync(userId);

            if (user == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "User not found.", 401, null, new List<string>()));
            }

            var response = await _authenticationService.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (response.Succeeded)
            {
                return Ok(new ApiResponse<string>(true, response.Message, response.StatusCode, null, new List<string>()));
            }
            else
            {
                return BadRequest(new ApiResponse<string>(false, response.Message, response.StatusCode, null, response.Errors));
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            return Ok(new ApiResponse<string>(true, "Logout successful", 200, null, new List<string>()));
        }
    }
}
