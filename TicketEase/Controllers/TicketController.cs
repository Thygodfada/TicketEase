using Microsoft.AspNetCore.Mvc;
using TicketEase.Application.DTO;
using TicketEase.Application.Interfaces.Services;
using TicketEase.Domain;
using TicketEase.Domain.Enums;

namespace TicketEase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpPost("add-ticket")]
        public IActionResult AddTicket(string userId, string projectId, [FromBody] TicketRequestDto ticketRequestDTO)
        {
            var response = _ticketService.AddTicket(userId, projectId, ticketRequestDTO);
            if (response.Succeeded)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }


        [HttpPut("edit-ticket/{ticketId}")]
        public IActionResult EditTicket(string ticketId, [FromBody] UpdateTicketRequestDto updatedTicketRequestDTO)
        {
            var response = _ticketService.EditTicket(ticketId, updatedTicketRequestDTO);
            if (response.Succeeded)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("user/{userId}")]
		public async Task<IActionResult> GetTicketsByUserId(string userId, int page, int perPage)
		{

			var result = await _ticketService.GetTicketByUserId(userId, page, perPage);
			return Ok(result);

		}

		[HttpGet("project/{projectId}")]
		public async Task<IActionResult> GetTicketsByProjectId(string projectId, int page, int perPage)
		{
			var result = await _ticketService.GetTicketByProjectId(projectId, page, perPage);
			return Ok(result);

		}

        [HttpDelete("{ticketId}")]
        public async Task<ActionResult<ApiResponse<bool>>>DeleteTicketById(string ticketId)
        {
            try
            {
                if (string.IsNullOrEmpty(ticketId))
                {
                    return BadRequest("Ticket ID is required.");
                }

                var response = await _ticketService.DeleteTicketByIdAsync(ticketId);

                if (response.Succeeded)
                {
                    return Ok(response);
                }

                return NotFound(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing the request{ex.Message}.");
            }
        }

        [HttpGet("status-by-pagination/{status}")]
        public async Task<IActionResult> GetTicketsByStatusWithPagination(Status status, int page, int pageSize)
        {
            try
            {

                if (page <= 0 || pageSize <= 0)
                {
                    return BadRequest("Invalid page or pageSize values.");
                }

                var result = await _ticketService.GetTicketsByStatusWithPagination(status, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing the request{ex.Message}");
            }
        }
        [HttpGet("All-tickets")]
        public async Task<IActionResult> GetAllTickets()
        {
            try
            {
                var result = await _ticketService.GetAllTickets();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing the request: {ex.Message}");
            }
        }
    }
}
