
using System.Linq.Expressions;
using TicketEase.Application.Interfaces.Repositories;
using TicketEase.Domain.Entities;
using TicketEase.Persistence.Context;

namespace TicketEase.Persistence.Repositories
{
	public class UserRepository : GenericRepository<AppUser>, IUserRepository
	{
		public UserRepository(TicketEaseDbContext ticketEaseDbContext): base(ticketEaseDbContext) { }

		public void AddUser(AppUser appUser) => Add(appUser);

		public void DeleteUser(AppUser appUser) => Delete(appUser);

        public List<AppUser> FindUser(Expression<Func<AppUser, bool>> condition)
        {
            var users = _ticketEaseDbContext.Set<AppUser>().AsQueryable().Where(condition).ToList();
            var filteredUsers = users.Where(u => u.Id != u.ManagerId).ToList();
            return filteredUsers;
        }


  //      public List<AppUser> FindUser(Expression<Func<AppUser, bool>> condition)
		//{
		//	return Find(condition);
		//}

		public List<AppUser> GetUser()
		{
			return GetAll();
		}

		public AppUser GetUserById(string id)
		{
			return GetById(id);
		}

		public void UpdateUser(AppUser appUser) => Update(appUser);


        public bool Exists(Expression<Func<AppUser, bool>> predicate)=> _ticketEaseDbContext.Set<AppUser>().Any(predicate);
    }
}
