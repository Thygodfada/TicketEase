using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TicketEase.Application.DTO;
using TicketEase.Application.Interfaces.Repositories;
using TicketEase.Application.Interfaces.Services;
using TicketEase.Common.Utilities;
using TicketEase.Domain;
using TicketEase.Domain.Entities;
//using TicketEase.Common.Utilities;

namespace TicketEase.Application.ServicesImplementation
{
    public class BoardServices : IBoardServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<BoardServices> _logger;

        public BoardServices(IUnitOfWork unitOfWork, IMapper mapper, ILogger<BoardServices> logger) 
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<BoardResponseDto>> AddBoardAsync(BoardRequestDto boardRequestDto)
        {
            ApiResponse<BoardResponseDto> response;

            try
            {   //TODD: Include Organization check in the Check for Board existence
                var existingBoard = _unitOfWork.BoardRepository.FindBoard(b => b.Name == boardRequestDto.Name).FirstOrDefault();
                if (existingBoard != null)
                {
                    response = new ApiResponse<BoardResponseDto>(false, StatusCodes.Status400BadRequest, $"Board already exists.");
                    return response;
                }

                //TODO: Check if organization exists
                var board = _mapper.Map<Board>(boardRequestDto);
                _unitOfWork.BoardRepository.AddBoard(board);
                _unitOfWork.SaveChanges();
                
                var responseDto = _mapper.Map<BoardResponseDto>(board);
                response = new ApiResponse<BoardResponseDto>(true, $"Successfully added a board", 201, responseDto, new List<string>());
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a board");
                var errorList = new List<string>();
                errorList.Add(ex.Message);
                response = new ApiResponse<BoardResponseDto>(true, "Error occurred while adding a board", 400, null, errorList);
                return response;
            }
        }

        public async Task<ApiResponse<GetBoardsDto>> GetBoardsByManagerId(string managerId, int perPage, int page)
        {
            try
            {
                var boards = _unitOfWork.BoardRepository.FindBoard(x=>x.ManagerId == managerId);

                var boardDtos = _mapper.Map<List<BoardResponseDto>>(boards);

                var pagedBoardDtos = await Pagination<BoardResponseDto>.GetPager(
                    boardDtos,
                    perPage,
                    page,
                    item => item.Name,
                    item => item.Id
                );

                var getBoardsDto = new GetBoardsDto
                {
                    Boards = pagedBoardDtos.Data.ToList(),
                    PerPage = pagedBoardDtos.PerPage,
                    CurrentPage = pagedBoardDtos.CurrentPage,
                    TotalPageCount = pagedBoardDtos.TotalPageCount,
                    TotalCount = pagedBoardDtos.TotalCount
                };

                return new ApiResponse<GetBoardsDto>(true, "Boards retrieved.", 200, getBoardsDto, new List<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all boards.");
                return new ApiResponse<GetBoardsDto>(false, "Error occurred while getting all boards.", 500, null, new List<string> { ex.Message });
            }
        }


        public async Task<ApiResponse<GetBoardsDto>> GetAllBoardsAsync(int perPage, int page)
        {
            try
            {
                var boards = _unitOfWork.BoardRepository.GetBoards();

                var boardDtos = _mapper.Map<List<BoardResponseDto>>(boards);

                var pagedBoardDtos = await Pagination<BoardResponseDto>.GetPager(
                    boardDtos,
                    perPage,
                    page,
                    item => item.Name,
                    item => item.Id
                );

                var getBoardsDto = new GetBoardsDto
                {
                    Boards = pagedBoardDtos.Data.ToList(),
                    PerPage = pagedBoardDtos.PerPage,
                    CurrentPage = pagedBoardDtos.CurrentPage,
                    TotalPageCount = pagedBoardDtos.TotalPageCount,
                    TotalCount = pagedBoardDtos.TotalCount
                };

                return new ApiResponse<GetBoardsDto>(true, "Boards retrieved.", 200, getBoardsDto, new List<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all boards.");
                return new ApiResponse<GetBoardsDto>(false, "Error occurred while getting all boards.", 500, null, new List<string> { ex.Message });
            }
        }
        
        

        public async Task<ApiResponse<BoardResponseDto>> GetBoardByIdAsync(string id)
        {
            try
            {
                var board = _unitOfWork.BoardRepository.GetBoardById(id);

                if (board == null)
                {
                    return new ApiResponse<BoardResponseDto>(false, "Board not found.", 404, null, new List<string>());
                }

                var boardDto = _mapper.Map<BoardResponseDto>(board);

                return new ApiResponse<BoardResponseDto>(true, "Board found.", 200, boardDto, new List<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting a board by ID.");
                return new ApiResponse<BoardResponseDto>(false, "Error occurred while getting a board by ID.", 500, null, new List<string> { ex.Message });
            }
        }
        public async Task<ApiResponse<BoardResponseDto>> UpdateBoardAsync(string boardId, BoardRequestDto boardRequestDto)
        {
            try
            {
                var existingBoard = _unitOfWork.BoardRepository.GetBoardById(boardId);
                if (existingBoard == null)
                    return await Task.FromResult(new ApiResponse<BoardResponseDto>(false, 400, $"Board not found."));

                var board = _mapper.Map(boardRequestDto, existingBoard);
                _unitOfWork.BoardRepository.UpdateBoard(existingBoard);
                _unitOfWork.SaveChanges();

                var responseDto = _mapper.Map<BoardResponseDto>(board);
                return await Task.FromResult(new ApiResponse<BoardResponseDto>(true, $"Successfully updated board", 200, responseDto, new List<string>()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating a board");
                var errorList = new List<string>();
                errorList.Add(ex.Message);
                return await Task.FromResult(new ApiResponse<BoardResponseDto>(true, "Error occurred while updating a board", 500, null, errorList));
            }
        }

        public ApiResponse<BoardResponseDto> DeleteAllBoards()
        {
            ApiResponse<BoardResponseDto> response;
            try
            {
                List<Board> boards = _unitOfWork.BoardRepository.GetBoards();
                _unitOfWork.BoardRepository.DeleteAllBoards(boards);
                response = new ApiResponse<BoardResponseDto>(true, StatusCodes.Status200OK, "All Boards deleted successfully");
                _unitOfWork.SaveChanges();
                return response;
            }
            catch (Exception ex)
            {
                response = new ApiResponse<BoardResponseDto>(false, StatusCodes.Status400BadRequest, "failed" + ex.InnerException);
                return response;
            }
        }

        public Task<ApiResponse<BoardResponseDto>> DeleteBoardAsync(string boardId)
        {
            try
            {
                var existingBoard = _unitOfWork.BoardRepository.GetBoardById(boardId);
                if (existingBoard == null)
                {
                    return Task.FromResult(new ApiResponse<BoardResponseDto>(false, 404, $"Board not found."));

                }
                _unitOfWork.BoardRepository.Delete(existingBoard);
                _unitOfWork.SaveChanges();
                return Task.FromResult(new ApiResponse<BoardResponseDto>(true, 200, $"Board deleted successfully ."));
            }
            catch (Exception)
            {

                return Task.FromResult(new ApiResponse<BoardResponseDto>(false, 500, $"An error occured during this process."));
            }
        }
    }
}
