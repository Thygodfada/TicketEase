using TicketEase.Application.DTO;
using TicketEase.Domain;
using TicketEase.Domain.Entities;

namespace TicketEase.Application.Interfaces.Services
{
    public interface IAuthenticationService
    {
		Task<ApiResponse<string>> RegisterAsync(AppUserCreateDto appUserCreateDto);
		Task<ApiResponse<string>> RegisterManagerAsync(AppUserCreateDto appUserCreateDto);
		Task<ApiResponse<string>> ConfirmEmailAsync(string email, string token);
		Task<ApiResponse<string>> LoginAsync(AppUserLoginDto loginDTO);
		Task<ApiResponse<string>> ForgotPasswordAsync(string email);
        Task<ApiResponse<string>> ResetPasswordAsync(string email, string token, string newPassword);
        Task<ApiResponse<string>> ValidateTokenAsync(string token);
        ApiResponse<string> ExtractUserIdFromToken(string authToken);
        Task<ApiResponse<string>> ChangePasswordAsync(AppUser user, string currentPassword, string newPassword);
    }
}
