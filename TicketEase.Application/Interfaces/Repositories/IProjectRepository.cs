
using System.Linq.Expressions;
using TicketEase.Domain.Entities;

namespace TicketEase.Application.Interfaces.Repositories
{
	public interface IProjectRepository : IGenericRepository<Project>
	{
		List<Project> GetProjects();
		void AddProject(Project project);
		void DeleteProject(Project project);
        void DeleteAllProjects(List<Project> projects);
        public List<Project> FindProject(Expression<Func<Project, bool>> condition);
		Project GetProjectById(string id);
		void UpdateProject(Project project);
	}
}
