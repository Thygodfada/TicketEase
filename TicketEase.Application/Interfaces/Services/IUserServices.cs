using Microsoft.AspNetCore.Mvc;
using TicketEase.Application.DTO;
using TicketEase.Application.DTO.Project;
using TicketEase.Common.Utilities;
using TicketEase.Domain;

namespace TicketEase.Application.Interfaces.Services
{
    public interface IUserServices
    {
        Task<ApiResponse<AppUserDto>> GetUserByIdAsync(string userId);
        Task<ApiResponse<PageResult<IEnumerable<AppUserDto>>>> GetUsersByPaginationAsync(int page, int perPage);
        Task<string> UpdateUserPhotoByUserId(string id, UpdatePhotoDTO model);
        Task<ApiResponse<bool>> UpdateUserAsync(string userId, UpdateUserDto updateUserDto);
        Task<ApiResponse<PageResult<IEnumerable<AppUserDto>>>> GetUsersByManagerIdAsync(string managerId, int page, int perPage);
    }
}
