using Microsoft.AspNetCore.Mvc;
using Serilog;
using TicketEase.Application.DTO;
using TicketEase.Application.DTO.Manager;
using TicketEase.Application.DTO.Project;
using TicketEase.Application.Interfaces.Services;
using TicketEase.Application.ServicesImplementation;
using TicketEase.Domain;

namespace TicketEase.Controllers
{
    [Route("api/managers")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly IManagerServices _managerService;
        private readonly IProjectServices _projectService;

        public ManagerController(IManagerServices managerService, IProjectServices projectService)
        {
            _managerService = managerService;
            _projectService = projectService;
        }

        [HttpPost("AddManager")]
        public async Task<IActionResult> CreateManager([FromBody] ManagerInfoCreateDto managerInfoCreateDto)
        {
            return Ok(await _managerService.CreateManager(managerInfoCreateDto));
        }


        [HttpGet("GetById")]
        public IActionResult GetManagersById(string id)
        {
            var response = _managerService.GetManagerById(id);
            return Ok(response);       
        }
        [HttpPut("Edit")]
        public IActionResult EditManager(string id, EditManagerDto managerDTO)
        {          
            var response = _managerService.EditManager(id, managerDTO);
            return Ok(response);            
        }
        [HttpGet("GetAll")]
        public IActionResult GetAllManage(int page, int perPage)
        {          
            var response = _managerService.GetAllManagerByPagination(page, perPage);
            return Ok(response);           
        }

        [HttpPost("sendManagerInformation")]
        public async Task<IActionResult> SendManagerInformation(ManagerInfoCreateDto managerInfoCreateDto)
        {    

                var response = await _managerService.SendManagerInformationToAdminAsync(managerInfoCreateDto);

                return Ok(response);            
        }

        [HttpPut("updateManager/{managerId}")]
        public async Task<IActionResult> UpdateManagerProfile(string managerId, [FromForm] UpdateManagerDto updateManagerDto)
        {
            var result = await _managerService.UpdateManagerProfileAsync(managerId, updateManagerDto);
            return Ok(new ApiResponse<bool>(true, "User updated successfully.", 200, true, null));              

           
        }

        [HttpGet("GetManagerDetails/{managerId}")]
        public async Task<IActionResult> GetManagerDetails(string managerId)
        {
            var boards = await _managerService.GetBoardsByManagerId(managerId);
            var projects = await _projectService.GetProjectsFromBoards(boards);
            var tickets = await _projectService.GetTicketsFromProjects(projects);

            var managerDetails = new
            {
                Boards = boards.Count,
                Projects = projects.Count,
                Tickets = tickets.Count
            };

            return Ok(managerDetails);
        }

        [HttpPatch("photo/{managerId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateManagerPhoto(string managerId, [FromForm] UpdatePhotoDTO updatePhotoDto)
        {
            var photoUpdateResult = await _managerService.UpdateManagerPhotoAsync(managerId, updatePhotoDto);

            if (photoUpdateResult.Succeeded)
            {
                return Ok(photoUpdateResult);
            }

           
            Log.Warning($"Failed to update photo for manager with ID {managerId}. {photoUpdateResult.Message}");

       
            return BadRequest(photoUpdateResult);
        }



    }
}
