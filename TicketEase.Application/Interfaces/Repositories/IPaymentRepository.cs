
using System.Linq.Expressions;
using TicketEase.Domain.Entities;

namespace TicketEase.Application.Interfaces.Repositories
{
	public interface IPaymentRepository : IGenericRepository<Payment>
	{
		List<Payment> GetPayments();
		void AddPayment(Payment payment);
		void DeletePayment(Payment payment);
		public List<Payment> FindPayment(Expression<Func<Payment, bool>> condition);
		Payment GetPaymentById(string id);
		void UpdatePayment(Payment payment);
	}
}
