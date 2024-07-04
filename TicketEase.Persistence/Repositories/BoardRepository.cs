using System.Linq.Expressions;
using TicketEase.Application.Interfaces.Repositories;
using TicketEase.Domain.Entities;
using TicketEase.Persistence.Context;

namespace TicketEase.Persistence.Repositories
{
	public class BoardRepository : GenericRepository<Board>, IBoardRepository
	{
		private readonly TicketEaseDbContext _ticketEaseDbContext;
		public BoardRepository(TicketEaseDbContext ticketEaseDbContext): base(ticketEaseDbContext)
		{
			_ticketEaseDbContext = ticketEaseDbContext;
		}

		public List<Board> GetBoards()
		{
			return GetAll();
		}
		public void AddBoard(Board board) => Add(board);

		public void DeleteBoard(Board board) => Delete(board);
		public List<Board> FindBoard(Expression<Func<Board, bool>> condition)
		{
			return Find(condition);
		}
		public Board GetBoardById(string id)
		{
			return GetById(id);
		}
		public void UpdateBoard(Board board) => Update(board);

        public void DeleteAllBoards(List<Board> boards)
        {
            DeleteAll(boards);
        }
    }
}
