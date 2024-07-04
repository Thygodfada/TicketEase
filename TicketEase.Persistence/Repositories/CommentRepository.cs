
using System.Linq.Expressions;
using TicketEase.Application.Interfaces.Repositories;
using TicketEase.Domain.Entities;
using TicketEase.Persistence.Context;

namespace TicketEase.Persistence.Repositories
{
	public class CommentRepository : GenericRepository<Comment>, ICommentRepository
	{
        public CommentRepository(TicketEaseDbContext ticketEaseDbContext) : base(ticketEaseDbContext) { }

		public void AddComment(Comment comment)=>Add(comment);

		public void DeleteComment(Comment comment)=>Delete(comment);

		public List<Comment> FindComment(Expression<Func<Comment, bool>> condition)
		{
			return Find(condition);
		}

		public Comment GetCommentById(string id)
		{
			return GetById(id);
		}

		public List<Comment> GetComments()
		{
			return GetAll();
		}

		public void UpdateComment(Comment comment)=>Update(comment);
	
	}
}
