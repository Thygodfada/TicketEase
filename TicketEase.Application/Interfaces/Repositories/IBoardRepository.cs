using System.Linq.Expressions;
using TicketEase.Domain.Entities;

namespace TicketEase.Application.Interfaces.Repositories
{
    public interface IBoardRepository : IGenericRepository<Board>
    {
        List<Board> GetBoards();
        void AddBoard(Board board);
        void DeleteBoard(Board board);
        void DeleteAllBoards(List<Board> boards);
        public List<Board> FindBoard(Expression<Func<Board, bool>> condition);
        Board GetBoardById(string id);
        void UpdateBoard(Board board);
    }
}
