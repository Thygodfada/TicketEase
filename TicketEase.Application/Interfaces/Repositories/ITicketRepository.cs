using System.Linq.Expressions;
using TicketEase.Common.Utilities;
using TicketEase.Domain.Entities;
using TicketEase.Domain.Enums;

namespace TicketEase.Application.Interfaces.Repositories
{
	public interface ITicketRepository : IGenericRepository<Ticket>
	{
		List<Ticket> GetTickets();
		void AddTicket(Ticket ticket);
		void DeleteTicket(Ticket ticket);
		public List<Ticket> FindTicket(Expression<Func<Ticket, bool>> condition);
		Task<List<Ticket>> GetTicketByProjectId(Expression<Func<Ticket, bool>> condition);
		Task<List<Ticket>> GetTicketByUserId(Expression<Func<Ticket, bool>> condition);
        Ticket GetTicketById(string id);
		void UpdateTicket(Ticket ticket);
        Task<PageResult<IEnumerable<Ticket>>> GetTicketsByStatusWithPagination(Status status, int page, int pageSize);
    }
}
