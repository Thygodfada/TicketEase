using TicketEase.Application.DTO;
using TicketEase.Common.Utilities;
using TicketEase.Domain;
using TicketEase.Domain.Entities;
using TicketEase.Domain.Enums;

namespace TicketEase.Application.Interfaces.Services
{
	public interface ITicketService
	{
		Task<ApiResponse<PageResult<IEnumerable<Ticket>>>> GetTicketByUserId(string userId, int page, int perPage);
		Task<ApiResponse<PageResult<IEnumerable<Ticket>>>> GetTicketByProjectId(string projectId, int page, int perPage);
        ApiResponse<TicketResponseDto> AddTicket(string userId, string ProjectId, TicketRequestDto ticketDTO);
        ApiResponse<TicketResponseDto> EditTicket(string ticketId, UpdateTicketRequestDto updatedTicketDTO);
        Task<ApiResponse<bool>> DeleteTicketByIdAsync(string ticketId);
        Task<PageResult<IEnumerable<Ticket>>> GetTicketsByStatusWithPagination(Status status, int page, int pageSize);
        Task<ApiResponse<IEnumerable<TicketResponseDto>>> GetAllTickets();
    }
}
