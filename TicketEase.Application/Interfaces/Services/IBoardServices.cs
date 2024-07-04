using TicketEase.Application.DTO;
using TicketEase.Domain;
using TicketEase.Domain.Entities;

namespace TicketEase.Application.Interfaces.Services
{
    public interface IBoardServices
    {
        Task<ApiResponse<BoardResponseDto>> AddBoardAsync(BoardRequestDto boardRequestDto);
        Task<ApiResponse<BoardResponseDto>> UpdateBoardAsync(string boardId, BoardRequestDto boardRequestDto);
        Task<ApiResponse<GetBoardsDto>> GetAllBoardsAsync(int PerPage, int Page);
        Task<ApiResponse<BoardResponseDto>> GetBoardByIdAsync(string id);
        ApiResponse<BoardResponseDto> DeleteAllBoards();
        Task<ApiResponse<GetBoardsDto>> GetBoardsByManagerId(string managerId, int perPage, int page);
        Task<ApiResponse<BoardResponseDto>> DeleteBoardAsync(string boardId);
    }
}
