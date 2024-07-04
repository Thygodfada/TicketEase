using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TicketEase.Application.DTO;
using TicketEase.Application.Interfaces.Repositories;
using TicketEase.Application.Interfaces.Services;
using TicketEase.Common.Utilities;
using TicketEase.Domain;
using TicketEase.Domain.Entities;
using TicketEase.Domain.Enums;

namespace TicketEase.Application.ServicesImplementation
{
    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TicketService> _logger;

        public TicketService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TicketService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public ApiResponse<TicketResponseDto> AddTicket(string userId, string projectId, TicketRequestDto ticketDTO)
        {
            ApiResponse<TicketResponseDto> response;

            if (string.IsNullOrEmpty(userId))
            {
                response = ApiResponse<TicketResponseDto>.Failed(new List<string> { "User ID is required." });
                return response;
            }

            if (string.IsNullOrEmpty(projectId))
            {
                response = ApiResponse<TicketResponseDto>.Failed(new List<string> { "Project ID is required." });
                return response;
            }

            try
            {
                var userExists = _unitOfWork.UserRepository.Exists(u => u.Id == userId);
                if (!userExists)
                {
                    response = ApiResponse<TicketResponseDto>.Failed(new List<string> { $"User with ID {userId} does not exist." });
                    return response;
                }

                var projectExists = _unitOfWork.ProjectRepository.Exists(p => p.Id == projectId);
                if (!projectExists)
                {
                    response = ApiResponse<TicketResponseDto>.Failed(new List<string> { $"Project with ID {projectId} does not exist." });
                    return response;
                }

                var ticketEntity = _mapper.Map<Ticket>(ticketDTO);
                ticketEntity.TicketReference = TicketHelper.GenerateTicketReference();
                ticketEntity.AppUserId = userId;
                ticketEntity.ProjectId = projectId;

                _unitOfWork.TicketRepository.AddTicket(ticketEntity);
                _unitOfWork.SaveChanges();

                var responseDto = _mapper.Map<TicketResponseDto>(ticketEntity);
                response = new ApiResponse<TicketResponseDto>(true, $"Successfully added a ticket", 201, responseDto, new List<string>());
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a ticket");
                return ApiResponse<TicketResponseDto>.Failed(new List<string> { "Error: " + ex.Message });
            }
        }




        public ApiResponse<TicketResponseDto> EditTicket(string ticketId, UpdateTicketRequestDto updatedTicketDTO)
        {
            ApiResponse<TicketResponseDto> response;
            try
            {
                var existingTicket = _unitOfWork.TicketRepository.GetTicketById(ticketId);

                if (existingTicket == null)
                {
                    _logger.LogWarning("Ticket not found while trying to edit");
                    return ApiResponse<TicketResponseDto>.Failed(new List<string> { "Ticket not found" });
                }

                _mapper.Map(updatedTicketDTO, existingTicket);

                _unitOfWork.TicketRepository.UpdateTicket(existingTicket);
                _unitOfWork.SaveChanges();

                var responseDto = _mapper.Map<TicketResponseDto>(existingTicket);
                response = new ApiResponse<TicketResponseDto>(true, $"Ticket updated successfully", StatusCodes.Status200OK, responseDto, new List<string>());
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while editing a ticket");
                return ApiResponse<TicketResponseDto>.Failed(new List<string> { "Error: " + ex.Message });
            }
        }
        public async Task<ApiResponse<PageResult<IEnumerable<Ticket>>>> GetTicketByProjectId(string projectId, int page, int perPage)
        {
            var tickets = await _unitOfWork.TicketRepository.GetTicketByProjectId(ticket => ticket.ProjectId == projectId);
            var pagedTickets = await Pagination<Ticket>.GetPager(
            tickets,
            perPage,
            page,
            ticket => ticket.Title,
            ticket => ticket.Id.ToString());


            //return pagedTickets;
            return new ApiResponse<PageResult<IEnumerable<Ticket>>>(true, "Operation succesful", StatusCodes.Status200OK, pagedTickets, new List<string>());
        }

        public async Task<ApiResponse<PageResult<IEnumerable<Ticket>>>> GetTicketByUserId(string userId, int page, int perPage)
        {
            var tickets = await _unitOfWork.TicketRepository.GetTicketByUserId(ticket => ticket.AppUserId == userId);

            // Use the Paginatioo paginate the dat
            var pagedTickets = await Pagination<Ticket>.GetPager(
                tickets,
                perPage,
                page,
                ticket => ticket.Title,
                ticket => ticket.Id.ToString());

            return new ApiResponse<PageResult<IEnumerable<Ticket>>>(true, "Operation succesful", 200, pagedTickets, new List<string>());
            //{
            //	Status = "Success",
            //	Data = pagedTickets
            //};

            //return pagedTickets;
        }

        public async Task<ApiResponse<bool>> DeleteTicketByIdAsync(string ticketId)
        {
            try
            {
                if (string.IsNullOrEmpty(ticketId))
                {
                    return ApiResponse<bool>.Failed(new List<string> { "Ticket ID is required." });
                }

                var existingTicket = _unitOfWork.TicketRepository.GetTicketById(ticketId);

                if (existingTicket == null)
                {
                    return ApiResponse<bool>.Failed(new List<string> { "Ticket not found." });
                }

                _unitOfWork.TicketRepository.DeleteTicket(existingTicket);
                _unitOfWork.SaveChanges();

                _logger.LogInformation($"Ticket with ID {ticketId} has been deleted successfully.");

                return ApiResponse<bool>.Success(true, "Ticket deleted successfully.", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the ticket.");

                return ApiResponse<bool>.Failed(new List<string> { "An error occurred while deleting the ticket." });
            }
        }

        public async Task<PageResult<IEnumerable<Ticket>>> GetTicketsByStatusWithPagination(Status status, int page, int pageSize)
        {
            try
            {
                var tickets = await _unitOfWork.TicketRepository.GetTicketsByStatusWithPagination(status, page, pageSize);

                _logger.LogInformation($"Retrieved {tickets.Data.Count()} tickets with status {status} (Page {page}, Page Size {pageSize}).");

                return tickets;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving tickets with pagination.");

                throw;
            }
        }
        public async Task<ApiResponse<IEnumerable<TicketResponseDto>>> GetAllTickets()
        {
            try
            {
                var tickets = _unitOfWork.TicketRepository.GetTickets();
                var ticketDtos = _mapper.Map<IEnumerable<TicketResponseDto>>(tickets);

                return ApiResponse<IEnumerable<TicketResponseDto>>.Success(ticketDtos, "Successfully retrieved all tickets.", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all tickets.");

                return ApiResponse<IEnumerable<TicketResponseDto>>.Failed(new List<string> { "An error occurred while retrieving all tickets." });
            }
        }
    }
}
