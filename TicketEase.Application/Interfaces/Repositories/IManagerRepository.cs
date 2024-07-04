
using System.Linq.Expressions;
using TicketEase.Domain.Entities;

namespace TicketEase.Application.Interfaces.Repositories
{
	public interface IManagerRepository : IGenericRepository<Manager>
	{
		List<Manager> GetManagers();
		void AddManager(Manager manager);
		void DeleteManager(Manager manager);
		public List<Manager> FindManager(Expression<Func<Manager, bool>> condition);
		Manager GetManagerById(string id);
		void UpdateManager(Manager manager);
	}
}
