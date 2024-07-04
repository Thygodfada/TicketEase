using Microsoft.AspNetCore.Mvc;
using TicketEase.Application.DTO;
using TicketEase.Application.DTO.Project;
using TicketEase.Application.Interfaces.Services;
using TicketEase.Domain;

namespace TicketEase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Route("api/users")]

    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserServices userServices, ILogger<UserController> logger)
        {
            _userServices = userServices;
            _logger = logger;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var response = await _userServices.GetUserByIdAsync(userId);

            if (response.Succeeded)
            {
                return Ok(response.Data);
            }

            return StatusCode(response.StatusCode, new { errors = response.Errors });
        }

        [HttpGet("get-Users-By-Pagination")]
        public async Task<IActionResult> GetUsersByPagination(int page, int perPage)
        {
            var response = await _userServices.GetUsersByPaginationAsync(page, perPage);

            if (response.Succeeded)
            {
                return Ok(response.Data);
            }

            return StatusCode(response.StatusCode, new { errors = response.Errors });
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto updateUserDto)
        {
            var updateResult = await _userServices.UpdateUserAsync(id, updateUserDto);

            if (updateResult.Succeeded)
            {
                return Ok(new ApiResponse<bool>(true, "User updated successfully.", 200, true, null));
            }

            _logger.LogError("User update failed: {Message}", updateResult.Message);
            return BadRequest(new ApiResponse<bool>(false, "Failed to update user.", 400, false, updateResult.Errors));
        }

        [HttpPatch("photo/{id}")]
        public async Task<IActionResult> UpdateUserPhotoByUserId(string id, [FromForm] UpdatePhotoDTO model)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return BadRequest("Invalid user ID");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var imageUrl = await _userServices.UpdateUserPhotoByUserId(id, model);

                if (imageUrl == null)
                    return NotFound($"User with ID {id} not found");

                return Ok(new { Url = imageUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user photo.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-Users-By-ManagerId")]
        public async Task<IActionResult> GetUsersByManagerIdAndPagination(string managerId, int page, int perPage)
        {
            var response = await _userServices.GetUsersByManagerIdAsync(managerId, page, perPage);

            if (response.Succeeded)
            {
                return Ok(response.Data);
            }

            return StatusCode(response.StatusCode, new { errors = response.Errors });
        }

    }
}

