using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketEase.Application.Interfaces.Services;

namespace TicketEase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TotalCountController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        IBoardServices _boardServices;
        IProjectServices _projectServices;

        public TotalCountController(ITicketService ticketService, IBoardServices boardServices, IProjectServices projectServices)
        {
            _ticketService = ticketService;
            _boardServices = boardServices;
            _projectServices = projectServices;
        }
        [HttpGet("GetTotalCounts")]
        public async Task<IActionResult> GetAllTickets()
        {
            try
            {
                var result = await _ticketService.GetAllTickets();
                var titcketCount = result.Data.ToList().Count;

                var result2 = await _boardServices.GetAllBoardsAsync(1, 3);
                var boardCount = result2.Data.TotalCount;

                var projectCount = _projectServices.GetAllProjects().Count;
                string[] counts = { titcketCount.ToString(), boardCount.ToString(), projectCount.ToString() };

                return Ok(counts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing the request: {ex.Message}");
            }
        }
    }
}
