
using System.Linq.Expressions;
using TicketEase.Application.Interfaces.Repositories;
using TicketEase.Domain.Entities;
using TicketEase.Persistence.Context;

namespace TicketEase.Persistence.Repositories
{
	public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
	{
		public PaymentRepository(TicketEaseDbContext ticketEaseDbContext) : base(ticketEaseDbContext) { }

		public void AddPayment(Payment payment) => Add(payment);

		public void DeletePayment(Payment payment) => Delete(payment);

		public List<Payment> FindPayment(Expression<Func<Payment, bool>> condition)
		{
			return Find(condition);
		}

		public Payment GetPaymentById(string id)
		{
			return GetById(id);
		}

		public List<Payment> GetPayments()
		{
			return GetAll();
		}

		public void UpdatePayment(Payment payment) => Update(payment);
	}
}
