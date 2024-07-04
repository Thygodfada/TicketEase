
using System.Linq.Expressions;
using TicketEase.Domain.Entities;

namespace TicketEase.Application.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepository<AppUser>
    {
		List<AppUser> GetUser();
		void AddUser(AppUser appUser);
		void DeleteUser(AppUser appUser);
		public List<AppUser> FindUser(Expression<Func<AppUser, bool>> condition);
		AppUser GetUserById(string id);
		void UpdateUser(AppUser appUser);
    }
}
