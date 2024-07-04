using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using TicketEase.Application.DTO;
using TicketEase.Application.DTO.Manager;
using TicketEase.Application.DTO.Project;
using TicketEase.Application.Interfaces.Repositories;
using TicketEase.Application.Interfaces.Services;
using TicketEase.Common.Utilities;
using TicketEase.Domain;
using TicketEase.Domain.Entities;

namespace TicketEase.Application.ServicesImplementation
{
	public class ManagerServices : IManagerServices
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly ILogger<ManagerServices> _logger;
		private readonly IEmailServices _emailServices;
		private readonly ICloudinaryServices<Manager> _cloudinaryServices;
		private readonly UserManager<AppUser> _userManager;
		private readonly IConfiguration _config;
		readonly IAuthenticationService _authenticationService;
		public ManagerServices(IAuthenticationService authenticationService, UserManager<AppUser> userManager, IConfiguration config, 
			IUnitOfWork unitOfWork, IMapper mapper, ILogger<ManagerServices> logger, IEmailServices emailServices, ICloudinaryServices<Manager> cloudinaryServices)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_logger = logger;
			_emailServices = emailServices;
			_cloudinaryServices = cloudinaryServices;
			_config = config;
			_userManager = userManager;
			_authenticationService = authenticationService;
		}

		public async Task<ApiResponse<ManagerResponseDto>> CreateManager(ManagerInfoCreateDto managerCreateDto)
		{
			var user = _unitOfWork.ManagerRepository.FindManager(x => x.BusinessEmail == managerCreateDto.BusinessEmail);
			if (user.Count > 0)
			{
				return new ApiResponse<ManagerResponseDto>(false, StatusCodes.Status400BadRequest, "Manager with this email already exist.");
			}

			string generatedPassword = PasswordGenerator.GeneratePassword(managerCreateDto.BusinessEmail, managerCreateDto.CompanyName);
			managerCreateDto.Password = generatedPassword;
			string emailBody = $"Welcome to TicketEase. An account has been created for you with the following details<br>Email: {managerCreateDto.BusinessEmail}, <br>Password: {generatedPassword}." +
				$"Follw this link to access your account http://localhost:3000/login";

			var manager = new Manager()
			{
				BusinessEmail = managerCreateDto.BusinessEmail,
				CompanyDescription = managerCreateDto.CompanyDescription,
				CompanyName = managerCreateDto.CompanyName
			};

			_unitOfWork.ManagerRepository.AddManager(manager);
			_unitOfWork.SaveChanges();

			var userCreateDto = new AppUserCreateDto()
			{
				ManagerId = manager.Id,
				Email = managerCreateDto.BusinessEmail,
				Password = generatedPassword
			};

			var response = await _authenticationService.RegisterManagerAsync(userCreateDto);
			if (response.Succeeded)
			{
				try
				{
					//await SendManagerInformationToAdminAsync(managerCreateDto);
					var mailRequest = new MailRequest
					{
						ToEmail = managerCreateDto.BusinessEmail,
						Subject = "Manager Information",
						Body = emailBody
					};
					await _emailServices.SendHtmlEmailAsync(mailRequest);

					var managerResponse = _mapper.Map<ManagerResponseDto>(manager);				
					return new ApiResponse<ManagerResponseDto>(true, response.Message, StatusCodes.Status200OK, managerResponse, new List<string>());
				}
				catch (Exception ex)
				{
					return new ApiResponse<ManagerResponseDto>(true, response.Message+". Unable to Send Email", StatusCodes.Status500InternalServerError, new List<string>() { ex.InnerException.ToString() });
				}
			}
			else
			{
				_unitOfWork.ManagerRepository.Delete(manager);
				_unitOfWork.SaveChanges();
				return new ApiResponse<ManagerResponseDto>(false, StatusCodes.Status500InternalServerError, response.Message);
			}

		}

		public Task<ApiResponse<EditManagerDto>> EditManager(string userId, EditManagerDto editManagerDto)
		{
			try
			{
				var existingManager = _unitOfWork.ManagerRepository.GetManagerById(userId);
				if (existingManager == null)
				{
					_logger.LogWarning("Manager with such Id does not exist");
					return Task.FromResult(new ApiResponse<EditManagerDto>(false, StatusCodes.Status400BadRequest, $"Manager not found."));
				}
				var manager = _mapper.Map(editManagerDto, existingManager);
				_unitOfWork.ManagerRepository.UpdateManager(existingManager);
				_unitOfWork.SaveChanges();
				var responseDto = _mapper.Map<EditManagerDto>(manager);
				_logger.LogInformation("Manager updated successfully");
				return Task.FromResult(new ApiResponse<EditManagerDto>(true, $"Successfully updated a Manager", 201, responseDto, new List<string>()));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while editing a Manager");
				var errorList = new List<string>();
				return Task.FromResult(new ApiResponse<EditManagerDto>(false, "Error occurred while adding a Manager", StatusCodes.Status400BadRequest, null, errorList));
			}
		}
		public async Task<ApiResponse<PageResult<IEnumerable<Manager>>>> GetAllManagerByPagination(int page, int perPage)
		{
			try
			{
				var managers = _unitOfWork.ManagerRepository.GetAll();

				var pagedManagers = await Pagination<Manager>.GetPager(
					managers,
					perPage,
					page,
					manager => manager.CompanyName,
					manager => manager.BusinessEmail);

				var response = new ApiResponse<PageResult<IEnumerable<Manager>>>(true, "Operation successful", StatusCodes.Status200OK,

					new PageResult<IEnumerable<Manager>>
					{
						Data = pagedManagers.Data.ToList(),
						TotalPageCount = pagedManagers.TotalPageCount,
						CurrentPage = pagedManagers.CurrentPage,
						PerPage = perPage,
						TotalCount = pagedManagers.TotalCount
					},

					new List<string>());
				return response;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while retrieving paged managers");
				return ApiResponse<PageResult<IEnumerable<Manager>>>.Failed(new List<string> { "Error: " + ex.Message });
			}
		}
		public ApiResponse<EditManagerDto> GetManagerById(string userId)
		{
			try
			{
				var existingManager = _unitOfWork.ManagerRepository.GetManagerById(userId);
				if (existingManager == null)
				{
					_logger.LogWarning("Manager with found ");
					return ApiResponse<EditManagerDto>.Failed(new List<string> { "Manager not found" });
				}
				var Manager = _mapper.Map<EditManagerDto>(existingManager);
				_logger.LogInformation("Manager retrieved successfully");
				return ApiResponse<EditManagerDto>.Success(Manager, "Manager retrieved successfully", StatusCodes.Status200OK);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while retrieving a Manager");
				return ApiResponse<EditManagerDto>.Failed(new List<string> { "Error: " + ex.Message });
			}
		}

        public async Task<ApiResponse<bool>> UpdateManagerProfileAsync(string managerId, UpdateManagerDto updateManagerDto)
        {
            try
            {
                var manager = _unitOfWork.ManagerRepository.GetManagerById(managerId);

                if (manager == null)
                {
                    return ApiResponse<bool>.Failed(false, "Manager not found.", StatusCodes.Status404NotFound, new List<string> { "Manager not found." });
                }

                var file = updateManagerDto.File;

                if (file != null)
                {
                    var photoUpdateResult = await UpdateManagerPhotoAsync(managerId, new UpdatePhotoDTO { PhotoFile = file });

                    if (!photoUpdateResult.Succeeded)
                    {
                        return ApiResponse<bool>.Failed(false, photoUpdateResult.Message, photoUpdateResult.StatusCode, photoUpdateResult.Errors);
                    }
                }

				// Update other properties
				//manager.UpdatedDate = DateTime.UtcNow;
				manager.BusinessEmail = updateManagerDto.BusinessEmail;
				manager.State = updateManagerDto.State;
				manager.BusinessPhone = updateManagerDto.BusinessPhone;
				manager.CompanyAddress = updateManagerDto.CompanyAddress;
				manager.CompanyName = updateManagerDto.CompanyName;

                // Update the manager entity in the repository
                _unitOfWork.ManagerRepository.UpdateManager(manager);

                // Save changes to the database
                _unitOfWork.SaveChanges();

                return ApiResponse<bool>.Success(true, "Manager updated successfully.", StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Failed(false, "Some error occurred.", StatusCodes.Status500InternalServerError, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<string>> UpdateManagerPhotoAsync(string managerId, UpdatePhotoDTO model)
        {
            if (model == null || model.PhotoFile == null || model.PhotoFile.Length <= 0)
            {
                Log.Warning($"Invalid file size for manager with ID {managerId}.");
                return ApiResponse<string>.Failed(false, $"Invalid file size for manager with ID {managerId}.", StatusCodes.Status400BadRequest, new List<string> { $"Invalid file size for manager with ID {managerId}." });
            }

            var manager = _unitOfWork.ManagerRepository.GetManagerById(managerId);

            if (manager == null)
            {
                Log.Warning($"Manager with ID {managerId} not found.");
                return ApiResponse<string>.Failed(false, $"Manager with ID {managerId} not found.", StatusCodes.Status404NotFound, new List<string> { $"Manager with ID {managerId} not found." });
            }

            // Upload the image to Cloudinary and get the URL
            var imageUrl = await _cloudinaryServices.UploadImage(managerId, model.PhotoFile);

            if (imageUrl == null)
            {
                Log.Warning($"Failed to upload image for manager with ID {managerId}.");
                return ApiResponse<string>.Failed(false, $"Failed to upload image for manager with ID {managerId}.", StatusCodes.Status500InternalServerError, new List<string> { "Failed to upload image." });
            }

            // Update the ImgUrl property with the Cloudinary URL
            manager.ImgUrl = imageUrl;

            // Update the manager entity in the repository
            _unitOfWork.ManagerRepository.UpdateManager(manager);

            _unitOfWork.SaveChanges();

            return ApiResponse<string>.Success(imageUrl, "Manager photo updated successfully.", StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<bool>> SendManagerInformationToAdminAsync(ManagerInfoCreateDto managerInfoCreateDto)
		{
			try
			{
				managerInfoCreateDto.AdminEmail = _config.GetSection("EmailSettings:Email").Value; ;
				var mailRequest = new MailRequest
				{
					ToEmail = managerInfoCreateDto.AdminEmail,
					Subject = "Manager Information",
					Body = $"An organisation requests to use TicketEase with the following details:<br>" +
					$"Business Email: {managerInfoCreateDto.BusinessEmail}<br>" +
						   $"Company Name: {managerInfoCreateDto.CompanyName}<br>" +
						   $"Company Description: {managerInfoCreateDto.CompanyDescription}"
				};
                await _emailServices.SendHtmlEmailAsync(mailRequest);
                return ApiResponse<bool>.Success(true, "Manager information sent to admin successfully", 200);
			}
			catch (Exception ex)
			{
				Log.Error(ex, "An error occurred while sending manager information to admin");
				return ApiResponse<bool>.Failed(new List<string> { "Error: " + ex.Message });
			}

		}		
						
		public string DeactivateManager(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return "Manager Id must be provided";
			}

			var manager = _unitOfWork.UserRepository.GetUserById(id);

			if (manager != null)
			{
				manager.IsActive = false;
				_unitOfWork.UserRepository.UpdateUser(manager);
				_unitOfWork.SaveChanges();// Save changes to deactivate

			}
			return $"Manager with Id {id} has been deactivated successfully";

		}

		public string ActivateManager(string id)
		{
			var manager = _unitOfWork.UserRepository.GetUserById(id);
			if (manager != null)
			{
				manager.IsActive = true;
				_unitOfWork.UserRepository.UpdateUser(manager);
				_unitOfWork.SaveChanges();// Save changes to deactivate
				return $"Manager with Id {id} has been activated successfully";
			}
			else
			{
				return "Manager not found";
			}


		}
        public async Task<List<Board>> GetBoardsByManagerId(string managerId)
        {
            return _unitOfWork.BoardRepository.FindBoard(x => x.ManagerId == managerId);
        }


    }
}
