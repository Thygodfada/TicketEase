using AutoMapper;
using Serilog;
using TicketEase.Application.DTO;
using TicketEase.Application.DTO.Project;
using TicketEase.Application.Interfaces.Repositories;
using TicketEase.Application.Interfaces.Services;
using TicketEase.Common.Utilities;
using TicketEase.Domain;
using TicketEase.Domain.Entities;

namespace TicketEase.Application.ServicesImplementation
{
    public class UserServices : IUserServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICloudinaryServices<AppUser> _cloudinaryServices;

        public UserServices(IUnitOfWork unitOfWork, IMapper mapper,
            ICloudinaryServices<AppUser> cloudinaryServices)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinaryServices = cloudinaryServices;
        }
        public async Task<ApiResponse<AppUserDto>> GetUserByIdAsync(string userId)
        {
            try
            {
                var user = _unitOfWork.UserRepository.GetUserById(userId);

                if (user == null)
                {
                    return ApiResponse<AppUserDto>.Failed(false, "User not found.", 404, new List<string> { "User not found." });
                }

                var userDto = _mapper.Map<AppUserDto>(user);

                return ApiResponse<AppUserDto>.Success(userDto, "User found.", 200);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while retrieving the user. UserID: {UserId}", userId);
                return ApiResponse<AppUserDto>.Failed(false, "An error occurred while retrieving the user.", 500, new List<string> { ex.Message });
            }

        }
        public async Task<ApiResponse<PageResult<IEnumerable<AppUserDto>>>> GetUsersByPaginationAsync(int page, int perPage)
        {
            try
            {
                var allUsers = _unitOfWork.UserRepository.GetAll();
                var pagedUsers = await Pagination<AppUser>.GetPager(
                    allUsers,
                    perPage,
                    page,
                    user => user.LastName,
                    user => user.Id.ToString()
                );
                var pagedUserDtos = _mapper.Map<PageResult<IEnumerable<AppUserDto>>>(pagedUsers);

                return ApiResponse<PageResult<IEnumerable<AppUserDto>>>.Success(pagedUserDtos, "Users found.", 200);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while retrieving users by pagination. Page: {Page}, PerPage: {PerPage}", page, perPage);
                return ApiResponse<PageResult<IEnumerable<AppUserDto>>>.Failed(false, "An error occurred while retrieving users by pagination.", 500, new List<string> { ex.Message });
            }
        }
        public async Task<ApiResponse<bool>> UpdateUserAsync(string userId, UpdateUserDto updateUserDto)
            {
            try
            {
                var user = _unitOfWork.UserRepository.GetUserById(userId);

                if (user == null)
                {
                    return ApiResponse<bool>.Failed(false, "User not found.", 404, new List<string> { "User not found." });
                }

                _mapper.Map(updateUserDto, user);

                _unitOfWork.UserRepository.UpdateUser(user);
                _unitOfWork.SaveChanges();

                return ApiResponse<bool>.Success(true, "User updated successfully.", 200);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while updating the user. UserID: {UserId}", userId);

                return ApiResponse<bool>.Failed(false, "An error occurred while updating the user.", 500, new List<string> { ex.Message });
            }
        }

        public async Task<string> UpdateUserPhotoByUserId(string id, UpdatePhotoDTO model)
        {
            try
            {
                var user = _unitOfWork.UserRepository.GetUserById(id);

                if (user == null)
                    return "User not found";

                var file = model.PhotoFile;

                if (file == null || file.Length <= 0)
                    return "Invalid file size";

                // Update other properties using AutoMapper
                _mapper.Map(model, user);

                // Upload the image to Cloudinary and get the URL
                var imageUrl = await _cloudinaryServices.UploadImage(id, file);

                if (imageUrl == null)
                {
                    Log.Warning($"Failed to upload image for user with ID {id}.");
                    return null;
                }

                // Update the ImageUrl property with the Cloudinary URL
                user.ImageUrl = imageUrl;

                // Update the user entity in the repository
                _unitOfWork.UserRepository.Update(user);

                // Save changes to the database
                _unitOfWork.SaveChanges();

                return imageUrl;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while updating user photo.");
                throw;
            }
        }


        public async Task<ApiResponse<PageResult<IEnumerable<AppUserDto>>>> GetUsersByManagerIdAsync(string managerId, int page, int perPage)
        {
            try
            {
                var users = _unitOfWork.UserRepository.FindUser(u => u.ManagerId == managerId).ToList();
                var filteredUsers = users.Where(u => u.Id != u.ManagerId).ToList();
                var pagedUsers = await Pagination<AppUser>.GetPager(
                    filteredUsers,
                    perPage,
                    page,
                    user => user.LastName,
                    user => user.Id.ToString()
                );
                var pagedUserDtos = _mapper.Map<PageResult<IEnumerable<AppUserDto>>>(pagedUsers);

                return ApiResponse<PageResult<IEnumerable<AppUserDto>>>.Success(pagedUserDtos, "Users found.", 200);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while retrieving users by manager ID and pagination. ManagerID: {ManagerId}", managerId);
                return ApiResponse<PageResult<IEnumerable<AppUserDto>>>.Failed(false, "An error occurred while retrieving users by manager ID and pagination.", 500, new List<string> { ex.Message });
            }
        }

    }
}


