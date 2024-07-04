using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TicketEase.Application.DTO;
using TicketEase.Application.DTO.Project;
using TicketEase.Application.Interfaces.Repositories;
using TicketEase.Application.Interfaces.Services;
using TicketEase.Common.Utilities;
using TicketEase.Domain;
using TicketEase.Domain.Entities;

namespace TicketEase.Application.ServicesImplementation
{
	public class ProjectServices : IProjectServices
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<ProjectServices> _logger;
		private readonly IMapper _mapper;

		public ProjectServices(IUnitOfWork unitOfWork, ILogger<ProjectServices> logger, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_logger = logger;
			_mapper = mapper;
		}

		public async Task<ApiResponse<ProjectReponseDto>> CreateProjectAsync(string id, ProjectRequestDto project)
		{
			ApiResponse<ProjectReponseDto> response;
		   
				var existingboard = _unitOfWork.BoardRepository.GetById(id);
				if (existingboard == null)
				{
				   response = new ApiResponse<ProjectReponseDto>(false, 404, $"Board with ID {id} not found.");
					return response;
				}

				var existingProject = _unitOfWork.ProjectRepository.FindProject(p => p.BoardId == id && p.Title == project.Title).FirstOrDefault();

				if (existingProject != null)
				{
				  response = new ApiResponse<ProjectReponseDto>(false, 400, $"Project with the same name already exists in the board.");
					return response;
				}
			try
			{

				var newProject = _mapper.Map<Project>(project);
				newProject.BoardId = id; // Set the board ID for the project

				_unitOfWork.ProjectRepository.AddProject(newProject);
				_unitOfWork.SaveChanges();

				var createdProject = _mapper.Map<ProjectReponseDto>(newProject);
			   
				response = ApiResponse<ProjectReponseDto>.Success(createdProject, $"Successfully created a project in the board:{id}", 201);
				return response;
			}
			catch (Exception ex)
			{

				_logger.LogError(ex, "Error occurred while adding a board");
				var errorList = new List<string>();
				errorList.Add(ex.Message);
				response = ApiResponse<ProjectReponseDto>.Failed(false, "Error occurred while creating a project in a board",500, new List<string> { ex.Message });
				return response;

			}
		}
		public List<ProjectReponseDto> GetAllProjects()
		{
			var projects = _unitOfWork.ProjectRepository.GetProjects();
			List<ProjectReponseDto> list = new();

			foreach (var project in projects)
			{
				var presponse = new ProjectReponseDto()
				{
					Description = project.Description,
					Id = project.Id,
					Title = project.Title,
				};
				list.Add(presponse);
			}

			return list;
		}

		public async Task<ApiResponse<GetProjectsDto>> GetAllProjectAsync(int perPage, int page)
		{
			try
			{
				var projects = _unitOfWork.ProjectRepository.GetProjects();

				var projectResponse = _mapper.Map<List<ProjectReponseDto>>(projects);

				var pagedProjectResponse = await Pagination<ProjectReponseDto>.GetPager(
					projectResponse,
					perPage,
					page,
					item => item.Title,
					item => item.Id
					);

				var getProjects = new GetProjectsDto
				{
					Projects = pagedProjectResponse.Data.ToList(),
					PerPage = pagedProjectResponse.PerPage,
					CurrentPage = pagedProjectResponse.CurrentPage,
					TotalPageCount = pagedProjectResponse.TotalPageCount,
					TotalCount = pagedProjectResponse.TotalCount
				};

				return new ApiResponse<GetProjectsDto>(true, "Projects Retrived.", 200, getProjects, new List<string>());
			}

			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while getting all projects.");
				return new ApiResponse<GetProjectsDto>(false, "Error occurred while getting all boards.", 500, null, new List<string> { ex.Message });
			}
		}

		public async Task<ApiResponse<ProjectReponseDto>> UpdateProjectAsync(/*string boardId, */string projectId, UpdateProjectRequestDto projectUpdate)
		{
			// Check if the board exists
			//var existingBoard =  _unitOfWork.BoardRepository.GetById(boardId);
			//if (existingBoard == null)
			//{
			//    return new ApiResponse<ProjectReponseDto>(false, StatusCodes.Status400BadRequest, $"Board with ID {boardId} not found.");

			//}

			// Check if the project exists
			var existingProject =  _unitOfWork.ProjectRepository.GetById(projectId);
			if (existingProject == null)
			{
				return new ApiResponse<ProjectReponseDto>(false, StatusCodes.Status400BadRequest, $"Board with ID {projectId} not found.");
			}

			try
			{
				// Update project properties based on projectUpdate
				existingProject.Title = projectUpdate.Title;
				existingProject.Description = projectUpdate.Description;

				_unitOfWork.ProjectRepository.UpdateProject(existingProject);
				 _unitOfWork.SaveChanges();

				var updatedProjectDto = _mapper.Map<ProjectReponseDto>(existingProject);
				return ApiResponse<ProjectReponseDto>.Success(updatedProjectDto, $"Successfully updated project with ID {projectId}", 200);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while updating a project");
				return ApiResponse<ProjectReponseDto>.Failed(false,"Error occurred while updating a project", 500, new List<string> { ex.Message });
			}




		}

		public async Task<ApiResponse<Project>> GetProjectByIdAsync(string projectId)
		{
			try
			{
				var project = _unitOfWork.ProjectRepository.GetProjectById(projectId);
				_logger.LogInformation("Project loaded successfully");

				return ApiResponse<Project>.Success(project, "Project loaded successfully", 200);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while loading the project");

				return ApiResponse<Project>.Failed(false, "Error occurred while loading the project", 500, new List<string> { ex.Message });
			}
		}

		public async Task<ApiResponse<PageResult<IEnumerable<Project>>>> GetProjectsByBoardIdAsync(string boardId, int perPage, int page)
		{
			try
			{
                var board = _unitOfWork.BoardRepository.GetById(boardId);

                if (board == null)
                {
                    
                    return ApiResponse<PageResult<IEnumerable<Project>>>.Failed(false, "Board not found", 404, new List<string> {});
                }
                var projects = _unitOfWork.ProjectRepository.GetAll();

				var boardProjects = projects.Where(project => project.BoardId == boardId).ToList();

				var paginationResponse = await Pagination<Project>.GetPager(boardProjects, perPage, page, p => p.Title, p => p.Id);

				return ApiResponse<PageResult<IEnumerable<Project>>>.Success(paginationResponse, "Successfully retrieved Projects", 200 );
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while loading the project");

				return ApiResponse<PageResult<IEnumerable<Project>>>.Failed(false, "Error occured whiile loading projects", 500, new List<string> {ex.Message});
			}
		}
		public ApiResponse<string> DeleteAllProjects()
		{
			ApiResponse<string> response;
			try
			{
				List<Project> projects = _unitOfWork.ProjectRepository.GetProjects();
				_unitOfWork.ProjectRepository.DeleteAllProjects(projects);
				response = new ApiResponse<string>("All Projects deleted successfully");
				_unitOfWork.SaveChanges();
				return response;
			}
			catch (Exception ex)
			{
				response = new ApiResponse<string>(false, StatusCodes.Status400BadRequest, "failed" + ex.InnerException);
				return response;
			}
		}

		public Task<ApiResponse<ProjectReponseDto>> DeleteProjectAsync(string projectId)
		{
			try
			{
				var existingproject = _unitOfWork.ProjectRepository.GetProjectById(projectId);
				if (existingproject == null)
				{
					return Task.FromResult(new ApiResponse<ProjectReponseDto>(false, 404, $"Project not found."));

				}
				_unitOfWork.ProjectRepository.Delete(existingproject);
				_unitOfWork.SaveChanges();
				return Task.FromResult(new ApiResponse<ProjectReponseDto>(true, 200, $"Project deleted successfully ."));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while while attempting to delete a project");
				return Task.FromResult(new ApiResponse<ProjectReponseDto>(false, 500, $"An error occured during this process."));
			}
		}
        public async Task<List<Project>> GetProjectsFromBoards(List<Board> boards)
        {
            List<Project> projectList = new();

            foreach (var board in boards)
            {
                var listofProjects = _unitOfWork.ProjectRepository.FindProject(x => x.BoardId == board.Id);
                foreach (var project in listofProjects)
                {
                    projectList.Add(project);
                }

            }
            return projectList;
        }
        public async Task<List<Ticket>> GetTicketsFromProjects(List<Project> projects)
        {
            List<Ticket> ticketList = new();
            foreach (var project in projects)
            {
                var listOfTickets = _unitOfWork.TicketRepository.FindTicket(x => x.ProjectId == project.Id);
                foreach (var ticket in listOfTickets)
                {
                    ticketList.Add(ticket);
                }
            }
            return ticketList;
        }
    }
}
