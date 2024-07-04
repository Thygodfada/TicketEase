using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TicketEase.Application.Interfaces.Repositories;
using TicketEase.Persistence.Context;

namespace TicketEase.Persistence.Repositories
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{
		protected readonly TicketEaseDbContext _ticketEaseDbContext;
		public GenericRepository(TicketEaseDbContext ticketEaseDbContext)
		{
			_ticketEaseDbContext = ticketEaseDbContext;
		}

		public void Add(T entity)=>_ticketEaseDbContext.Set<T>().Add(entity);

		public void DeleteAll(List<T> entity) => _ticketEaseDbContext.Set<T>().RemoveRange(entity);

		public void Delete(T entity)=> _ticketEaseDbContext.Set<T>().Remove(entity);

		public List<T> Find(System.Linq.Expressions.Expression<Func<T, bool>> predicate)=>_ticketEaseDbContext.Set<T>().Where(predicate).ToList();

		public List<T> GetAll()=>_ticketEaseDbContext.Set<T>().ToList();

		public T GetById(string id)=>_ticketEaseDbContext.Set<T>().Find(id);

		public void Update(T entity)=>_ticketEaseDbContext.Set<T>().Update(entity);
        public void SaveChanges() => _ticketEaseDbContext.SaveChanges();
        public bool Exists(Expression<Func<T, bool>> predicate) =>  _ticketEaseDbContext.Set<T>().Any(predicate);
        
    }
}
