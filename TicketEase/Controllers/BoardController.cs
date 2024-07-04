using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketEase.Application.DTO;
using TicketEase.Application.Interfaces.Services;
using TicketEase.Application.ServicesImplementation;
using TicketEase.Domain;

namespace TicketEase.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BoardController : ControllerBase
	{
		private readonly IBoardServices _boardServices;
		public BoardController(IBoardServices boardServices)
		{
			_boardServices = boardServices;
		}

		//[Authorize(Roles = "Admin, Manager")]
		[HttpPost("AddBoard")]
		public async Task<IActionResult> AddBoard([FromBody] BoardRequestDto request)
			=>  Ok(await _boardServices.AddBoardAsync(request));

	
		
		[AllowAnonymous]
		//[Authorize(Roles = "Admin,Manager")]
		[HttpPut("UpdateBoard/{boardId}")]
		public async Task<IActionResult> UpdateBoard(string boardId, [FromBody] BoardRequestDto request)
		{
			return Ok(await _boardServices.UpdateBoardAsync(boardId, request));
		}

		[HttpGet("GetBoardById/{id}")]
		public async Task<ActionResult<ApiResponse<BoardResponseDto>>> GetBoardById(string id)
		{
			return Ok(await _boardServices.GetBoardByIdAsync(id));
		}

        [HttpGet("GetBoardByManagerId")]
        public async Task<ActionResult<ApiResponse<BoardResponseDto>>> GetBoardByManagerId(string managerId,  int page,  int perPage)
        {
            return Ok(await _boardServices.GetBoardsByManagerId(managerId,page,perPage));
        }

        [HttpGet("get-all-board-by-pagination")]
		public async Task<IActionResult> GetAllBoards([FromQuery] int page, [FromQuery] int perPage)
		{
			try
			{
				var result = await _boardServices.GetAllBoardsAsync(perPage, page);

				if (result.Succeeded)
				{
					return Ok(result.Data);
				}
				else
				{
					return StatusCode(result.StatusCode, new { Message = result.Message, Errors = result.Errors });
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { Message = "Internal Server Error", Errors = new[] { ex.Message } });
			}
		}
	   

		[HttpDelete("DeleteBoard")]
		public IActionResult DeleteBoard()
		{
			return Ok(_boardServices.DeleteAllBoards());
		}

		[HttpDelete("DeleteBoardById")]
		public async Task<IActionResult> DeleteBoardById(string boardId)
		{
			return Ok(await _boardServices.DeleteBoardAsync(boardId));
		}
	}
}
