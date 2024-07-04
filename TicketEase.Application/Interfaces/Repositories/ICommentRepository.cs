
using System.Linq.Expressions;
using TicketEase.Domain.Entities;

namespace TicketEase.Application.Interfaces.Repositories
{
	public interface ICommentRepository : IGenericRepository<Comment>
	{
		List<Comment> GetComments();
		void AddComment(Comment comment);
		void DeleteComment(Comment comment);
		public List<Comment> FindComment(Expression<Func<Comment, bool>> condition);
		Comment GetCommentById(string id);
		void UpdateComment(Comment comment);
	}
}
